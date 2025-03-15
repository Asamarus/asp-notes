using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a note sources reorder request.
/// </summary>
public class NotesSourcesReorderRequest
{
    /// <summary>
    /// Gets or sets the list of note sources IDs in the desired order.
    /// </summary>
    [Required]
    public required List<string> SourceIds { get; set; }
}