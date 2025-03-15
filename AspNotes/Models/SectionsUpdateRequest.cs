using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request to update an existing section.
/// </summary>
public sealed class SectionsUpdateRequest
{
    /// <summary>
    /// Gets or sets the display name of the section.
    /// </summary>
    [Required]
    public required string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the color of the section.
    /// </summary>
    [Required]
    [RegularExpression(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", ErrorMessage = "Color must be a valid #hex color.")]
    public required string Color { get; set; }
}
