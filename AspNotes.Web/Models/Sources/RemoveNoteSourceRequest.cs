using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Sources;

public class RemoveNoteSourceRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "NoteId must be a positive number!")]
    public long NoteId { get; set; }

    [Required]
    [RegularExpression(@"\S", ErrorMessage = "SourceId must not be empty or whitespace.")]
    public string SourceId { get; set; } = null!;
}