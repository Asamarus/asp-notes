using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Notes;

public class UpdateNoteBookRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number!")]
    public long Id { get; set; }

    [Required(AllowEmptyStrings = true)]
    public string Book { get; set; } = null!;
}