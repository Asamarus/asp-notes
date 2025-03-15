using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNotes.Entities;

/// <summary>
/// Represents a section entity with properties for name, display name, color, and position.
/// </summary>
[Table("Sections")]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Position))]
public class SectionEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the section.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the section.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the display name of the section.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the color associated with the section.
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Gets or sets the position of the section.
    /// </summary>
    public required uint Position { get; set; }
}