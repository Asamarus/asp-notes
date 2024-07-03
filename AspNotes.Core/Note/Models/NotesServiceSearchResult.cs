namespace AspNotes.Core.Note.Models;

public class NotesServiceSearchResult
{
    public List<NoteDto> Rows { get; set; } = [];

    public int Total { get; set; }

    public int Count { get; set; }

    public int LastPage { get; set; }

    public bool LoadMore { get; set; } = false;

    public int Page { get; set; }

    public string? SearchTerm { get; set; }

    public HashSet<string> Keywords { get; set; } = [];

    public bool FoundWholePhrase { get; set; } = false;
}
