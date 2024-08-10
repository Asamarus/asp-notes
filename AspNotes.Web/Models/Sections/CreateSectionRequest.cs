using System.ComponentModel.DataAnnotations;

namespace AspNotes.Web.Models.Sections;

public class CreateSectionRequest
{
    [Required]
    [StringLength(20, MinimumLength = 4)]
    [RegularExpression(@"^[a-z]+$", ErrorMessage = "Name must contain only Latin letters and no whitespaces!")]
    public string Name { get; set; } = null!;

    [Required]
    public string DisplayName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", ErrorMessage = "Color must be a valid #hex color!")]
    public string Color { get; set; } = null!;
}