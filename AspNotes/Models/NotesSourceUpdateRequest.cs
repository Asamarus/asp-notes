using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request to update an existing note source.
/// </summary>
public class NotesSourceUpdateRequest
{
    /// <summary>
    /// Gets or sets the link associated with the note source.
    /// </summary>
    [Required]
    [Url(ErrorMessage = "The Link is not a valid URL.")]
    public required string Link { get; set; }

    /// <summary>
    /// Gets or sets the title of the note source.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the note source.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the image URL of the note source.
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the image.
    /// </summary>
    public bool ShowImage { get; set; } = false;
}
