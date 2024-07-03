namespace AspNotes.Core.Note.Models;

public class NotesServiceSearchRequest
{

    public string? Section { get; set; }

    public string? SearchTerm { get; set; }

    public int Page { get; set; } = 1;

    public int ResultsPerPage { get; set; } = 50;

    public string? Book { get; set; }

    public List<string> Tags { get; set; } = [];

    public bool InRandomOrder { get; set; } = false;

    public bool WithoutBook { get; set; } = false;

    public bool WithoutTags { get; set; } = false;

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }
}
