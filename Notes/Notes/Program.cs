using Microsoft.EntityFrameworkCore;
using Notes.Persistence;

var notesDbPath = Path.Join(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "notes.db"
);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<NotesContext>(o => o.UseSqlite($"Data Source={notesDbPath}"));

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
