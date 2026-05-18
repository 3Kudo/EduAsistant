namespace EduAsistant.Models
{
    public class CalendarDayViewModel
    {
        public DateTime Date { get; set; }

        public List<CalendarSessionViewModel> Sessions { get; set; } = new();
    }
}
