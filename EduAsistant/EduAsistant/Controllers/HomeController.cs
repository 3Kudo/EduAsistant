using System.Diagnostics;
using EduAsistant.Data;
using EduAsistant.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduAsistant.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Student> _userManager;

        // Wstrzykujemy bazê danych oraz mened¿era u¿ytkowników (Identity)
        public HomeController(AppDbContext context, UserManager<Student> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Jeœli ktoœ nie jest zalogowany, dajemy mu pust¹ listê (lub mo¿na go przekierowaæ na stronê logowania)
            if (!User.Identity!.IsAuthenticated)
            {
                return View(new List<Subject>());
            }

            // 1. Pobieramy ID aktualnie zalogowanego u¿ytkownika (Twojego admina)
            var userId = _userManager.GetUserId(User);

            // 2. Pobieramy TYLKO kursy tego u¿ytkownika (plus sylabusy, ¿eby policzyæ chapters)
            var userSubjects = await _context.Subjects
                .Include(s => s.Syllabuses)
                .ThenInclude(sy => sy.Topics)
                .Where(s => s.StudentId == userId) // <--- Magia: filtrujemy po ID u¿ytkownika!
                .ToListAsync();

            // 3. Przekazujemy gotow¹ listê do widoku z Figmy
            return View(userSubjects);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}