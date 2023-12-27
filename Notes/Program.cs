using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Notes.Areas.Identity.Data;
using Notes.Persistence;

internal class Program
{
    private static void Main(string[] args)
    {
        var notesDbPath = Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "notes.db"
        );
        var connectionString = $"Data Source={notesDbPath}";
        Console.WriteLine($"Using database at {notesDbPath}");

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<NotesContext>(o => o.UseSqlite(connectionString));
        builder.Services.AddDbContext<NotesIdentityDbContext>(o => o.UseSqlite(connectionString));

        builder
            .Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<NotesIdentityDbContext>();

        var app = builder.Build();
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Notes/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.MapControllerRoute(name: "default", pattern: "{controller=Notes}/{action=Index}/{id?}");
        app.MapRazorPages();

        using var serviceScope = app.Services.CreateScope();
        using var db = serviceScope.ServiceProvider.GetService<NotesContext>()!;
        if (!db.NoteTypes.Any())
        {
            db.NoteTypes.Add(new NoteType { Name = "Work" });
            db.NoteTypes.Add(new NoteType { Name = "Personal" });
            db.NoteTypes.Add(new NoteType { Name = "Other" });
            db.SaveChanges();
        }

        app.Run();
    }
}
