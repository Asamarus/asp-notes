using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request containing details about a note item.
/// </summary>
public sealed class NotesPatchRequest
{
    /// <summary>
    /// Gets or sets the section of the note item.
    /// </summary>
    [MinLength(1, ErrorMessage = "Section cannot be an empty.")]
    public string? Section { get; set; }

    /// <summary>
    /// Gets or sets the book associated with the note item, if any.
    /// </summary>
    public string? Book { get; set; }

    /// <summary>
    /// Gets or sets the tags associated with the note item.
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Gets or sets the title of the note item.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the note item.
    /// </summary>
    public string? Content { get; set; }
}

