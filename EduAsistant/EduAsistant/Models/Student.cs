using Microsoft.AspNetCore.Identity;

namespace EduAsistant.Models
{
    public class Student : IdentityUser
    {
        public TimeSpan DailyAvailableStart { get; set; }
        public TimeSpan DailyAvailableEnd { get; set; }
        public List<Subject> Subjects { get; set; } = new();
        public List<BlockOutDate> BlockOutDates { get; set; } = new();
    }
}