namespace AspNotes.Entities;

/// <summary>
/// Represents the base entity with common properties.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
