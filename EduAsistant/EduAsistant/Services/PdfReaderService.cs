using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace EduAsistant.Services
{
    public class PdfReaderService
    {
        public string ExtractTextFromPdf(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return string.Empty;
            }

            StringBuilder textBuilder = new StringBuilder();

            using (PdfDocument document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    var text = ContentOrderTextExtractor.GetText(page);
                    textBuilder.AppendLine(text);
                }
            }

            return textBuilder.ToString();
        }
    }
}