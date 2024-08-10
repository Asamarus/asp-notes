using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Notes;

public class UpdateNoteSectionRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number!")]
    public long Id { get; set; }

    [Required]
    public string Section { get; set; } = null!;
}