using AspNotes.Core.Note.Models;

namespace AspNotes.Web.Models.Notes;

public class NoteItemResponse
{
    public long Id { get; set; }

    public string CreatedAt { get; set; } = null!;

    public string UpdatedAt { get; set; } = null!;

    public string? Title { get; set; }

    public string Section { get; set; } = null!;

    public string? Content { get; set; }

    public string? Preview { get; set; }

    public string? Book { get; set; }

    public List<string> Tags { get; set; } = [];

    public List<NoteSource> Sources { get; set; } = [];

    public NoteItemResponse() { }

    public NoteItemResponse(NoteDto note)
    {
        Id = note.Id;
        CreatedAt = note.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        UpdatedAt = note.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        Title = note.Title;
        Section = note.Section;
        Content = note.Content;
        Preview = note.Preview;
        Book = note.Book;
        Tags = new List<string>(note.Tags);
        Sources = note.Sources.Select(source => new NoteSource(source)).ToList();
    }
}
