using AspNotes.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AspNotes.Core.Section.Models;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Position))]
public class SectionEntity : EntityBase
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string DisplayName { get; set; } = null!;

    [Required]
    public string Color { get; set; } = null!;

    [Required]
    public uint Position { get; set; }
}
