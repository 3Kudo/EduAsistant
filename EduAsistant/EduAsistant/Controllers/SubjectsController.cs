using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EduAsistant.Data;
using EduAsistant.Models;
using EduAsistant.Services;

namespace EduAsistant.Controllers
{
    [Authorize]
    public class SubjectsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Student> _userManager;
        private readonly PdfReaderService _pdfReader;
        private readonly AiAnalyzerService _aiAnalyzer;
        private readonly StudyPlannerService _studyPlanner;

        public SubjectsController(AppDbContext context, UserManager<Student> userManager,
                PdfReaderService pdfReader, AiAnalyzerService aiAnalyzer, StudyPlannerService studyPlanner)
        {
            _context = context;
            _userManager = userManager;
            _pdfReader = pdfReader;
            _aiAnalyzer = aiAnalyzer;
            _studyPlanner = studyPlanner;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var mySubjects = await _context.Subjects
                .Where(s => s.StudentId == userId)
                .ToListAsync();
            return View(mySubjects);
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            var subject = await _context.Subjects
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id && m.StudentId == userId);

            if (subject == null) return NotFound();

            return View(subject);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ExamDate")] Subject subject, IFormFile? syllabusFile)
        {
            var userId = _userManager.GetUserId(User);
            subject.StudentId = userId;
            ModelState.Remove("StudentId");
            ModelState.Remove("Student");
            ModelState.Remove("Syllabuses");
            ModelState.Remove("SyllabusFileName");

            if (ModelState.IsValid)
            {
                if (syllabusFile != null && syllabusFile.Length > 0)
                {
                    // A. Zapis pliku na dysku
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(syllabusFile.FileName);
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(uploadPath)!);

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await syllabusFile.CopyToAsync(stream);
                    }
                    subject.SyllabusFileName = fileName;

                    // B. Ekstrakcja tekstu z PDF
                    string extractedText = _pdfReader.ExtractTextFromPdf(uploadPath);

                    // C. Analiza AI 
                    try
                    {
                        string aiJsonResult = await _aiAnalyzer.AnalyzeSyllabusAsync(extractedText);

                        // D. Deserializacja JSON-a od AI na listę tematów
                        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var aiTopics = System.Text.Json.JsonSerializer.Deserialize<List<AiTopicResult>>(aiJsonResult, options);


                        if (aiTopics != null && aiTopics.Any())
                        {
                            // E. Obiekt Sylabus z tematami
                            var newSyllabus = new Syllabus()
                            {
                                Title = $"Sylabus: {subject.Name}",
                                FilePath = fileName,
                            };

                            foreach (var aiTopic in aiTopics)
                            {
                                newSyllabus.Topics.Add(new Topic
                                {
                                    Name = aiTopic.Name,
                                    TotalEstimatedMinutes = aiTopic.TotalEstimatedMinutes
                                });
                            }

                            // Przypisywanie sylabusa do przedmiotu
                            subject.Syllabuses.Add(newSyllabus);
                            var sessions = _studyPlanner.GenerateStudySessions(
                                newSyllabus.Topics.ToList(),
                                DateTime.Now.AddDays(1),
                                 subject.ExamDate
                    );
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("BŁĄD AI: " + ex.Message);
                    }
                }

                // 3. Zapis do bazy (Przedmiot + Sylabus + Tematy)
                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine("BŁĄD WALIDACJI: " + error.ErrorMessage);
            }

            return View(subject);
        }


        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id && s.StudentId == userId);

            if (subject == null) return NotFound();

            return View(subject);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Credits,SyllabusFileName")] Subject subject)
        {
            if (id != subject.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            subject.StudentId = userId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id && m.StudentId == userId);

            if (subject == null) return NotFound();

            return View(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id && s.StudentId == userId);

            if (subject != null)
            {
                if (!string.IsNullOrEmpty(subject.SyllabusFileName))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", subject.SyllabusFileName);
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }

                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(int id)
        {
            var userId = _userManager.GetUserId(User);
            return _context.Subjects.Any(e => e.Id == id && e.StudentId == userId);
        }
    }

    // Klasa pomocnicza do odczytywania odpowiedzi z AI
    public class AiTopicResult
    {
        public string Name { get; set; } = string.Empty;
        public int TotalEstimatedMinutes { get; set; }
    }
}