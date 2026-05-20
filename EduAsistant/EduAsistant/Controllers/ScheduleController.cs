using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EduAsistant.Data;
using EduAsistant.Models;

namespace EduAsistant.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Student> _userManager;

        public ScheduleController(AppDbContext context, UserManager<Student> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var userId = _userManager.GetUserId(User);

            var sessions = await _context.StudySessions
                .Include(ss => ss.Topic)
                    .ThenInclude(t => t.Syllabus)
                        .ThenInclude(s => s.Subject)
                .Where(ss => ss.Topic != null && ss.Topic.Syllabus != null && ss.Topic.Syllabus.Subject != null)
                .Where(ss => ss.Topic.Syllabus.Subject.StudentId == userId)
                .ToListAsync();

            var events = sessions.Select(ss => new
            {
                id = ss.Id,
                title = !string.IsNullOrEmpty(ss.CustomTitle) ? ss.CustomTitle : (ss.Topic?.Name ?? "Wydarzenie"),
                start = ss.ScheduledDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = ss.ScheduledDate.AddMinutes(ss.DurationMinutes > 0 ? ss.DurationMinutes : 60).ToString("yyyy-MM-ddTHH:mm:ss"),
                color = ss.Topic?.Syllabus?.Subject?.Color ?? "#8B5CF6",
                allDay = false,
                extendedProps = new { isCompleted = ss.IsCompleted }
            });

            return Json(events);
        }
    }
}