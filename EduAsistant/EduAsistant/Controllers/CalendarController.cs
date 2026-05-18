using EduAsistant.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduAsistant.Models;

namespace EduAsistant.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Student> _userManager;

        public CalendarController(
            AppDbContext context,
            UserManager<Student> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var sessions = await _context.StudySessions
                .Include(s => s.Topic)
                    .ThenInclude(t => t.Syllabus)
                        .ThenInclude(sy => sy.Subject)
                .Where(s =>
                    s.Topic != null &&
                    s.Topic.Syllabus.Subject.StudentId == userId)
                .OrderBy(s => s.ScheduledDate)
                .ToListAsync();

            var groupedDays = sessions
                .GroupBy(s => s.ScheduledDate.Date)
                .Select(day => new CalendarDayViewModel
                {
                    Date = day.Key,

                    Sessions = day.Select(s => new CalendarSessionViewModel
                    {
                        Id = s.Id,
                        TopicName = s.Topic!.Name,
                        SubjectName = s.Topic.Syllabus.Subject.Name,
                        ScheduledDate = s.ScheduledDate,
                        DurationMinutes = s.DurationMinutes,
                        IsCompleted = s.IsCompleted,
                        SubjectColor = s.Topic.Syllabus.Subject.Color
                    }).ToList()
                })
                .OrderBy(d => d.Date)
                .ToList();

            return View(groupedDays);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var session = await _context.StudySessions.FindAsync(id);

            if (session == null)
                return NotFound();

            session.IsCompleted = !session.IsCompleted;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}