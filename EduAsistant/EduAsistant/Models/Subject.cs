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
    }
}