namespace AspNotes.Models;

/// <summary>
/// Represents a generic paginated response containing a list of items and pagination details.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
public sealed class PaginatedResponse<T>
{
    /// <summary>
    /// Gets or sets the total number of items.
    /// </summary>
    public required long Total { get; set; }

    /// <summary>
    /// Gets or sets the count of items in the current response.
    /// </summary>
    public required long Count { get; set; }

    /// <summary>
    /// Gets or sets the last page number.
    /// </summary>
    public required int LastPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether more items can be loaded.
    /// </summary>
    public required bool CanLoadMore { get; set; }

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public required int Page { get; set; }

    /// <summary>
    /// Gets or sets the search term used in the query.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the keywords associated with the search.
    /// </summary>
    public HashSet<string> Keywords { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the whole phrase was found.
    /// </summary>
    public bool FoundWholePhrase { get; set; } = false;

    /// <summary>
    /// Gets or sets the list of items.
    /// </summary>
    public required IEnumerable<T> Data { get; set; }
}