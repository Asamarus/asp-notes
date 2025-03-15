namespace AspNotes.Models;

/// <summary>
/// Represents the response containing application data.
/// </summary>
public sealed class ApplicationDataResponse
{
    /// <summary>
    /// Gets or sets the section containing all notes.
    /// </summary>
    public required AllNotesSection AllNotesSection { get; set; }

    /// <summary>
    /// Gets or sets the list of section item responses.
    /// </summary>
    public required IEnumerable<SectionsItemResponse> Sections { get; set; }
}
