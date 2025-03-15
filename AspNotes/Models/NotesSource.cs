namespace AspNotes.Models;

/// <summary>
/// Represents the source of a note, including its link, title, description, and image.
/// </summary>
public sealed class NotesSource
{
    /// <summary>
    /// Gets or sets the unique identifier for the NoteSource.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the link associated with the NoteSource.
    /// </summary>
    public required string Link { get; set; }

    /// <summary>
    /// Gets or sets the title of the NoteSource.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the NoteSource.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the image associated with the NoteSource.
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the image should be shown.
    /// </summary>
    public bool ShowImage { get; set; } = false;
}
