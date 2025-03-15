namespace AspNotes.Models;

/// <summary>
/// Represents a response containing an item name and its count.
/// </summary>
public sealed class ItemNameCountResponse
{
    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the count of the item.
    /// </summary>
    public required long Count { get; set; }
}
