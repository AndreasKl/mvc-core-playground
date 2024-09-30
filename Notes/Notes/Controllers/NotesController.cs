using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.Persistence;

namespace Notes.Controllers;

public class NotesController(NotesContext notesContext) : Controller
{
    public record NotesViewModel(List<NoteViewModel> Notes, AddNoteForm AddNoteForm);

    public record NoteViewModel(Guid ID, string Title, string Description, string Type);

    public record AddNoteForm(
        List<NoteTypeViewModel>? NoteTypes,
        [Required] string? Title,
        [Required] string? Description,
        [Required] Guid? NoteTypeID
    );

    public record NoteTypeViewModel(Guid ID, string Name);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(
            new NotesViewModel(
                await FetchNotes(),
                new AddNoteForm(await FetchNoteTypes(), "", "", null)
            )
        );
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromForm] AddNoteForm form)
    {
        if (!ModelState.IsValid)
        {
            var clone = form with { NoteTypes = await FetchNoteTypes() };
            return View(new NotesViewModel(await FetchNotes(), clone));
        }

        await notesContext.Notes.AddAsync(
            new Note { Description = form.Description!, Title = form.Title!, TypeID = form.NoteTypeID!.Value }
        );
        await notesContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var note = await notesContext.Notes.FindAsync(id);
        if (note is null)
        {
            return NotFound();
        }

        notesContext.Notes.Remove(note);
        await notesContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }

    private async Task<List<NoteViewModel>> FetchNotes()
    {
        return await notesContext
            .Notes.Select(n => new NoteViewModel(n.ID, n.Title, n.Description, n.Type.Name))
            .ToListAsync();
    }

    private async Task<List<NoteTypeViewModel>> FetchNoteTypes()
    {
        return await notesContext
            .NoteTypes.Select(nt => new NoteTypeViewModel(nt.ID, nt.Name))
            .ToListAsync();
    }
}
