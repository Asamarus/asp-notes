namespace AspNotes.Core.Note.Models;

public class NoteSource
{
    public string Id { get; set; } = null!;

    public string Link { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public bool ShowImage { get; set; } = false;
}
