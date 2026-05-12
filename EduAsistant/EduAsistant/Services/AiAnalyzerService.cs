using System.Text;
using System.Text.Json;

namespace EduAsistant.Services
{
    public class AiAnalyzerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        public AiAnalyzerService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiApiKey"];
        }

        public async Task<string> AnalyzeSyllabusAsync(string syllabusText)
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={_apiKey}";
            string prompt = @"You are an educational assistant. Analyze the syllabus below and extract a list of study topics. 
For each topic, estimate the time (in MINUTES) an average student needs to dedicate to master it.
Return the response EXCLUSIVELY as a raw JSON array of objects. Do not add any other text, do not use any Markdown formatting, and do not use code blocks. 
The JSON format must look exactly like this:
[{""Name"": ""Introduction to Algebra"", ""TotalEstimatedMinutes"": 120}, {""Name"": ""Quadratic Equations"", ""TotalEstimatedMinutes"": 180}]

Syllabus text: " + syllabusText;

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, jsonContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            using var jsonDocument = JsonDocument.Parse(responseString);
            var extractedJsonString = jsonDocument.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return extractedJsonString ?? "[]";
        }
    }
}