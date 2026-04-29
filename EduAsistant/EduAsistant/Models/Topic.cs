namespace EduAsistant.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalEstimatedMinutes { get; set; }
        public int SyllabusId { get; set; }
        public Syllabus Syllabus { get; set; } = null!;
        public List<StudySession> StudySessions { get; set; } = new();
    }
}