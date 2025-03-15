using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNotes.Entities;

/// <summary>
/// Represents a tag entity with properties for section and name.
/// </summary>
[Table("Tags")]
[Index(nameof(Section))]
[Index(nameof(Name))]
public class TagEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the tag.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the section to which the tag belongs.
    /// </summary>
    public required string Section { get; set; }

    /// <summary>
    /// Gets or sets the name of the tag.
    /// </summary>
    public required string Name { get; set; }
}
