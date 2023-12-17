namespace Notes.Persistence;

public class Note
{
    public Guid ID { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid TypeID { get; set; }
    public NoteType Type { get; set; } = null!;
}
