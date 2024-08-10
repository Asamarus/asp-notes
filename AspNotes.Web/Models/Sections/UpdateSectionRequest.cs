using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Sections;

public class UpdateSectionRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number!")]
    public long Id { get; set; }

    [Required]
    public string DisplayName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", ErrorMessage = "Color must be a valid #hex color!")]
    public string Color { get; set; } = null!;
}