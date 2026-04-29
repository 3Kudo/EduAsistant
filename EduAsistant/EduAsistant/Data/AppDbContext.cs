using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EduAsistant.Models;

namespace EduAsistant.Data
{
    public class AppDbContext : IdentityDbContext<Student>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BlockOutDate> BlockOutDates { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Syllabus> Syllabuses { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<StudySession> StudySessions { get; set; }
    }
}