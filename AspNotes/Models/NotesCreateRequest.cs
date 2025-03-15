using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request to create a new note.
/// </summary>
public sealed class NotesCreateRequest
{
    /// <summary>
    /// Gets or sets the section of the note.
    /// </summary>
    [Required]
    public required string Section { get; set; }

    /// <summary>
    /// Gets or sets the book associated with the note.
    /// </summary>
    public string? Book { get; set; }
}