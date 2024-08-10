namespace AspNotes.Core.Note.Models;

public class NoteDto
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Title { get; set; }

    public string Section { get; set; } = null!;

    public string? Content { get; set; }

    public string? Preview { get; set; }

    public string? Book { get; set; }

    public List<string> Tags { get; set; } = [];

    public List<NoteSource> Sources { get; set; } = [];

    public NoteDto() { }

    public NoteDto(NoteEntity note)
    {
        Id = note.Id;
        CreatedAt = note.CreatedAt;
        UpdatedAt = note.UpdatedAt;
        Title = note.Title;
        Section = note.Section;
        Content = note.Content;
        Preview = note.Preview;
        Book = note.Book;
        Tags = new List<string>(note.Tags);
        Sources = note.Sources.Select(source => new NoteSource(source)).ToList();
    }
}
