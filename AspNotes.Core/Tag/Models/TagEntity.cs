using AspNotes.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AspNotes.Core.Tag.Models;

[Index(nameof(Section))]
[Index(nameof(Name))]
public class TagEntity : EntityBase
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string Section { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;
}
