using AspNotes.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AspNotes.Core.Note.Models;

[Index(nameof(Section))]
[Index(nameof(Book))]
public class NoteEntity : EntityBase
{
    [Key]
    public long Id { get; set; }

    public string? Title { get; set; }

    public string? TitleSearchIndex { get; set; }

    [Required]
    public string Section { get; set; } = null!;

    public string? Content { get; set; }

    public string? ContentSearchIndex { get; set; }

    public string? Preview { get; set; }

    public string? Book { get; set; }

    public List<string> Tags { get; set; } = [];

    public List<NoteSource> Sources { get; set; } = [];
}
