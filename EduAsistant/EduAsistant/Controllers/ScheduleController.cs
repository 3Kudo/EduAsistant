using EduAsistant.Data;
using EduAsistant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduAsistant.Controllers
{
    // [Authorize] sprawia, że tylko zalogowani użytkownicy wejdą w kalendarz
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

        // 1. Wyświetla widok kalendarza (stronę HTML)
        public IActionResult Index()
        {
            return View();
        }

        // 2. Ten "ukryty" punkt będzie wywoływany przez nasz kalendarz, żeby pobrać zadania
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var userId = _userManager.GetUserId(User);

            // Wyciągamy z bazy tylko sesje należące do zalogowanego studenta
            var sessions = await _context.StudySessions
                .Include(ss => ss.Topic)
                    .ThenInclude(t => t.Syllabus)
                        .ThenInclude(s => s.Subject)
                .Where(ss => ss.Topic.Syllabus.Subject.StudentId == userId)
                .ToListAsync();

            // Transformujemy to na format, który zrozumie kalendarz FullCalendar.js
            var events = sessions.Select(ss => new
            {
                id = ss.Id,
                title = ss.Topic.Name, // Tytuł to nazwa tematu z sylabusa
                start = ss.ScheduledDate.ToString("yyyy-MM-ddTHH:mm:ss"), // np. "2026-05-20T14:30:00"
                end = ss.ScheduledDate.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss"), // zakładamy 1h na sesję
                color = ss.Topic.Syllabus.Subject.Color ?? "#8B5CF6", // Pobiera kolor kursu z Figmy!
                allDay = false
            });

            return Json(events);
        }
    }
}