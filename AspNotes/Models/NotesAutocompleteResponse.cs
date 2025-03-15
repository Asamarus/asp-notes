namespace AspNotes.Models;

/// <summary>
/// Represents a response containing autocomplete suggestions for notes, books, and tags.
/// </summary>
public sealed class NotesAutocompleteResponse
{
    /// <summary>
    /// Gets or sets the list of note <see cref="NotesAutocompleteResultItem"/>.
    /// </summary>
    public required List<NotesAutocompleteResultItem> Notes { get; set; }

    /// <summary>
    /// Gets or sets the list of book suggestions.
    /// </summary>
    public required List<string> Books { get; set; }

    /// <summary>
    /// Gets or sets the list of tag suggestions.
    /// </summary>
    public required List<string> Tags { get; set; }
}
