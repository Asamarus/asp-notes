using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNotes.Entities;

/// <summary>
/// Represents a full-text search entity for notes.
/// </summary>
public class NoteFtsEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the full-text search entity.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the note.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the note.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the full-text search match string.
    /// </summary>
    [Column("NotesFTS")]
    public string Match { get; set; } = null!;

    /// <summary>
    /// Gets or sets the rank of the full-text search match.
    /// </summary>
    public double? Rank { get; set; }

    /// <summary>
    /// Gets or sets the note entity associated with the full-text search entity.
    /// </summary>
    public NoteEntity Note { get; set; } = null!;
}
