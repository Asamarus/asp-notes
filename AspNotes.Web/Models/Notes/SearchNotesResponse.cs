namespace AspNotes.Web.Models.Notes;

public class SearchNotesResponse
{
    public List<NoteItemResponse> Notes { get; set; } = [];

    public int Total { get; set; }

    public int Count { get; set; }

    public int LastPage { get; set; }

    public bool CanLoadMore { get; set; } = false;

    public int Page { get; set; }

    public string? SearchTerm { get; set; }

    public HashSet<string> Keywords { get; set; } = [];

    public bool FoundWholePhrase { get; set; } = false;
}