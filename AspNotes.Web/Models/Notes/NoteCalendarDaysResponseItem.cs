namespace AspNotes.Web.Models.Notes;

public class NoteCalendarDaysResponseItem
{
    public required long Count { get; set; }

    public required DateOnly Date { get; set; }
}