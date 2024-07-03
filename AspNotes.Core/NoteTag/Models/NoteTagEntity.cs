using AspNotes.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNotes.Core.NoteTag.Models;

[Table("NotesTags")]
[Index(nameof(NoteId))]
[Index(nameof(TagId))]
[Index(nameof(NoteId), nameof(TagId), IsUnique = true)]
public class NoteTagEntity : EntityBase
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long NoteId { get; set; }

    [Required]
    public long TagId { get; set; }
}