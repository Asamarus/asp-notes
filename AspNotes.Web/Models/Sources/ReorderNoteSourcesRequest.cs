using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Sources;

public class ReorderNoteSourcesRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "NoteId must be a positive number!")]
    public long NoteId { get; set; }

    [Required]
    public List<string> SourceIds { get; set; } = null!;
}