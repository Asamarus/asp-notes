using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request to get notes with various filtering and pagination options.
/// </summary>
public sealed class NotesGetRequest
{
    /// <summary>
    /// Gets or sets the section to filter the notes.
    /// </summary>
    public string? Section { get; set; }

    /// <summary>
    /// Gets or sets the search term to filter the notes.
    /// Must be at least three symbols long.
    /// </summary>
    [MinLength(3, ErrorMessage = "SearchTerm must be at least three symbols long.")]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the page number for pagination.
    /// Must be a positive integer larger or equal to 1.
    /// Default value is 1.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be a positive integer larger or equal to 1.")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the book to filter the notes.
    /// </summary>
    public string? Book { get; set; }

    /// <summary>
    /// Gets or sets the tags to filter the notes.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether to return the notes in random order.
    /// Default value is false.
    /// </summary>
    public bool InRandomOrder { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to filter out notes with a book.
    /// Default value is false.
    /// </summary>
    public bool WithoutBook { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to filter out notes with tags.
    /// Default value is false.
    /// </summary>
    public bool WithoutTags { get; set; } = false;

    /// <summary>
    /// Gets or sets the start date to filter the notes.
    /// </summary>
    public DateOnly? FromDate { get; set; }

    /// <summary>
    /// Gets or sets the end date to filter the notes.
    /// </summary>
    public DateOnly? ToDate { get; set; }
}