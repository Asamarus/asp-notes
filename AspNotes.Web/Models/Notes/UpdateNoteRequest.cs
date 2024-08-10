using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Notes;

public class UpdateNoteRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number!")]
    public long Id { get; set; }

    [Required(AllowEmptyStrings = true)]
    public string Title { get; set; } = null!;

    [Required(AllowEmptyStrings = true)]
    public string Content { get; set; } = null!;
}