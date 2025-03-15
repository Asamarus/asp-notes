namespace AspNotes.Models;

/// <summary>
/// Represents metadata for an HTML document.
/// </summary>
public sealed class HtmlDocumentMetadata
{
    /// <summary>
    /// Gets or sets the title of the HTML document.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the HTML document.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the image associated with the HTML document.
    /// </summary>
    public string? Image { get; set; }
}
