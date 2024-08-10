namespace AspNotes.Web.Models.Notes;

public class AutocompleteNotesResponse
{
    public List<AutoCompleteNotesItemResponse> Notes { get; set; } = [];

    public HashSet<string> Books { get; set; } = [];

    public HashSet<string> Tags { get; set; } = [];
}

public class AutoCompleteNotesItemResponse
{
    public required long Id { get; set; }

    public required string Title { get; set; }
}