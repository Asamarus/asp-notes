namespace AspNotes.Models;

/// <summary>
/// Represents an item in the notes autocomplete result.
/// </summary>
public sealed class NotesAutocompleteResultItem
{
    /// <summary>
    /// Gets or sets the unique identifier of the note.
    /// </summary>
    public required long Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the note.
    /// </summary>
    public required string Title { get; set; }
}
