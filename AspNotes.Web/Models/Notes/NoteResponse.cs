namespace AspNotes.Web.Models.Notes;

public class NoteResponse
{
    public string? Message { get; set; }

    public bool ShowNotification { get; set; } = false;

    public required NoteItemResponse Note { get; set; }
}
