using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request to create a new note source.
/// </summary>
public sealed class NotesSourceCreateRequest
{
    /// <summary>
    /// Gets or sets the link associated with the note source.
    /// </summary>
    [Required]
    [Url(ErrorMessage = "The Link is not a valid URL!")]
    public required string Link { get; set; }
}
