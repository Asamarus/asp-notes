using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Sources;

public class AddNoteSourceRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "NoteId must be a positive number!")]
    public long NoteId { get; set; }

    [Required]
    [Url(ErrorMessage = "The Link is not a valid URL!")]
    public string Link { get; set; } = null!;
}