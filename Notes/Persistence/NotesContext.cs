using Microsoft.EntityFrameworkCore;

namespace Notes.Persistence;

public class NotesContext(DbContextOptions<NotesContext> dbContextOptions)
    : DbContext(dbContextOptions)
{
    public DbSet<Note> Notes { get; set; }
    public DbSet<NoteType> NoteTypes { get; set; }
}
