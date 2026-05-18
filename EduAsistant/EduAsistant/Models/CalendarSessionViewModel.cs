namespace EduAsistant.Models
{
    public class CalendarSessionViewModel
    {
        public int Id { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;

        public DateTime ScheduledDate { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsCompleted { get; set; }

        public string SubjectColor { get; set; } = "#8B5CF6";
    }
}
