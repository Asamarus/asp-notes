using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request to create a new section.
/// </summary>
public sealed class SectionsCreateRequest
{
    /// <summary>
    /// Gets or sets the name of the section.
    /// </summary>
    [Required]
    [StringLength(20, MinimumLength = 2)]
    [RegularExpression(@"^[a-z0-9_]+$", ErrorMessage = "Name must contain only lowercase Latin letters, numbers, underscores and no whitespaces.")]
    public required string Name { get; set; }

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