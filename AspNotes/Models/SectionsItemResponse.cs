using AspNotes.Entities;

namespace AspNotes.Models;

/// <summary>
/// Represents a response containing details about a section.
/// </summary>
public sealed class SectionsItemResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the section.
    /// </summary>
    public required long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the section.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the display name of the section.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the color of the section.
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Converts a <see cref="SectionEntity"/> to a <see cref="SectionsItemResponse"/>.
    /// </summary>
    /// <param name="section">The section entity to convert.</param>
    /// <returns>A <see cref="SectionsItemResponse"/> representing the section entity.</returns>
    public static SectionsItemResponse FromEntity(SectionEntity section)
    {
        return new SectionsItemResponse
        {
            Id = section.Id,
            Name = section.Name,
            DisplayName = section.DisplayName,
            Color = section.Color,
        };
    }
}
