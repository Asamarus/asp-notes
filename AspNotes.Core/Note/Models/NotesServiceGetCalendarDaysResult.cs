namespace AspNotes.Core.Note.Models;

public class NotesServiceGetCalendarDaysResult
{
    public required long Count { get; set; }

    public required DateOnly Date { get; set; }
}
