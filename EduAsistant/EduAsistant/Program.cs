using EduAsistant.Data;
using EduAsistant.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<Student>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<EduAsistant.Services.PdfReaderService>();
builder.Services.AddHttpClient<EduAsistant.Services.AiAnalyzerService>();
builder.Services.AddScoped<EduAsistant.Services.StudyPlannerService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Student>>();

    if (!userManager.Users.Any())
    {
        var admin = new Student
        {
            UserName = "admin@edu.pl",
            Email = "admin@edu.pl",
            EmailConfirmed = true,
            DailyAvailableStart = new TimeSpan(8, 0, 0),
            DailyAvailableEnd = new TimeSpan(20, 0, 0)
        };

        var result = userManager.CreateAsync(admin, "Admin123!").Result;

        if (result.Succeeded)
        {
            Console.WriteLine("Stworzono");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

