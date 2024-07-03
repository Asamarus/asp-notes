namespace AspNotes.Core.Note.Models;

public class NotesServiceGetCalendarDaysResult
{
    public required long Number { get; set; }

    public required DateOnly Date { get; set; }
}
