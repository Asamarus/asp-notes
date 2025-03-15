namespace AspNotes.Models;

/// <summary>
/// Represents a response item containing the count of notes and the date.
/// </summary>
public sealed class NotesCalendarDaysResponseItem
{
    /// <summary>
    /// Gets or sets the count of notes for the specified date.
    /// </summary>
    public required long Count { get; set; }

    /// <summary>
    /// Gets or sets the date for the note count.
    /// </summary>
    public required string Date { get; set; }
}
