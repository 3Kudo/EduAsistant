namespace EduAsistant.Models
{
    public class StudySession
    {
        public int Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsCompleted { get; set; }
        public string? CustomTitle { get; set; }
        public int? TopicId { get; set; }
        public Topic? Topic { get; set; }
    }
}