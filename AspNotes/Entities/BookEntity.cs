using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNotes.Entities;

/// <summary>
/// Represents a book entity with properties for section and name.
/// </summary>
[Table("Books")]
[Index(nameof(Section))]
[Index(nameof(Name))]
public class BookEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the book.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the section to which the book belongs.
    /// </summary>
    public required string Section { get; set; }

    /// <summary>
    /// Gets or sets the name of the book.
    /// </summary>
    public required string Name { get; set; }
}
