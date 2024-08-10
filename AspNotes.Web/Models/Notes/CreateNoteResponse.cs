namespace AspNotes.Web.Models.Notes;

public class CreateNoteResponse
{
    public required string Message { get; set; }

    public required NoteItemResponse Note { get; set; }
}