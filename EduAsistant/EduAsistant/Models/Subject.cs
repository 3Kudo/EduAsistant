namespace EduAsistant.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public Student Student { get; set; } = null!;
        public List<Syllabus> Syllabuses { get; set; } = new();
        public string? SyllabusFileName { get; set; }
        public string Color { get; set; } = "#8B5CF6"; // Domyślnie ten fioletowy
        public double DailyStudyLimit { get; set; } = 2.0; // Limit w godzinach
        public string Priority { get; set; } = "Medium"; // Low, Medium, Hard
    }
}