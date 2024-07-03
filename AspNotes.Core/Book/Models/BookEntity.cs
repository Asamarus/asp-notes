using AspNotes.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace AspNotes.Core.Book.Models;

[Index(nameof(Section))]
[Index(nameof(Name))]
public class BookEntity : EntityBase
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string Section { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;
}
