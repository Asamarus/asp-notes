namespace AspNotes.Models;

/// <summary>
/// Represents a section containing all notes.
/// </summary>
public sealed class AllNotesSection
{
    /// <summary>
    /// Gets or sets the name of the section.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the display name of the section.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the color of the section.
    /// </summary>
    public required string Color { get; set; }
}