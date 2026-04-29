namespace EduAsistant.Models
{
    public class Syllabus
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
        public List<Topic> Topics { get; set; } = new();
    }
}