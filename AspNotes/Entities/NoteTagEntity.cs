using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNotes.Entities;

/// <summary>
/// Represents the association between a Note and a Tag.
/// </summary>
[Table("NotesTags")]
[Index(nameof(NoteId))]
[Index(nameof(TagId))]
[Index(nameof(NoteId), nameof(TagId), IsUnique = true)]
public class NoteTagEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the NoteTagEntity.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the associated Note.
    /// </summary>
    public long NoteId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the associated Tag.
    /// </summary>
    public long TagId { get; set; }
}
