using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Sources;

public class UpdateNoteSourceRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "NoteId must be a positive number!")]
    public long NoteId { get; set; }

    [Required]
    [RegularExpression(@"\S", ErrorMessage = "SourceId must not be empty or whitespace.")]
    public string SourceId { get; set; } = null!;

    [Required]
    [Url(ErrorMessage = "The Link is not a valid URL.")]
    public string Link { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public bool ShowImage { get; set; } = false;

}