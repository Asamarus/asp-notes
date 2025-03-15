using AspNotes.Entities;

namespace AspNotes.Models;

/// <summary>
/// Represents a response containing details about a note item.
/// </summary>
public sealed class NotesItemResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the note item.
    /// </summary>
    public required long Id { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the note item.
    /// </summary>
    public required string CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date of the note item.
    /// </summary>
    public required string UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the section of the note item.
    /// </summary>
    public required string Section { get; set; }

    /// <summary>
    /// Gets or sets the title of the note item.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the note item.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the preview of the note item.
    /// </summary>
    public string? Preview { get; set; }

    /// <summary>
    /// Gets or sets the book associated with the note item, if any.
    /// </summary>
    public string? Book { get; set; }

    /// <summary>
    /// Gets or sets the tags associated with the note item.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets the sources associated with the note item.
    /// </summary>
    public List<NotesSource> Sources { get; set; } = [];

    /// <summary>
    /// Converts a <see cref="NoteEntity"/> to a <see cref="NotesItemResponse"/>.
    /// </summary>
    /// <param name="entity">The note entity to convert.</param>
    /// <returns>A <see cref="NotesItemResponse"/> representing the section entity.</returns>
    public static NotesItemResponse FromEntity(NoteEntity entity)
    {
        return new NotesItemResponse
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = entity.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            Section = entity.Section,
            Title = entity.Title,
            Content = entity.Content,
            Preview = entity.Preview,
            Book = entity.Book,
            Tags = entity.TagsList,
            Sources = entity.Sources,
        };
    }
}