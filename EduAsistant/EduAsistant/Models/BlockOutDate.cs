namespace EduAsistant.Models
{
    public class BlockOutDate
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Reason { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public Student Student { get; set; } = null!;
    }
}