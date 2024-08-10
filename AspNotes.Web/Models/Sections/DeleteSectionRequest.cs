using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Sections;

public class DeleteSectionRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number!")]
    public long Id { get; set; }
}