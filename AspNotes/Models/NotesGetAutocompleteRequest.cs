using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request to get autocomplete suggestions for notes.
/// </summary>
public sealed class NotesGetAutocompleteRequest
{
    /// <summary>
    /// Gets or sets the search term for the autocomplete request.
    /// </summary>
    [Required]
    [MinLength(2, ErrorMessage = "Search term must be at least 2 characters long.")]
    public required string SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the section of the note.
    /// </summary>
    public string? Section { get; set; }

    /// <summary>
    /// Gets or sets the book associated with the note.
    /// </summary>
    public string? Book { get; set; }
}