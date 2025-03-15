using AspNotes.Helpers;
using AspNotes.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNotes.Entities;

/// <summary>
/// Represents a note entity with various properties including title, section, content, tags, and sources.
/// </summary>
[Table("Notes")]
[Index(nameof(Section))]
[Index(nameof(Book))]
public class NoteEntity : BaseEntity
{
    private List<string> tagsList = [];
    private string tags = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the NoteEntity.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the note.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the search index for the title.
    /// </summary>
    public string? TitleSearchIndex { get; set; }

    /// <summary>
    /// Gets or sets the section of the note.
    /// </summary>
    public required string Section { get; set; }

    /// <summary>
    /// Gets or sets the content of the note.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the search index for the content.
    /// </summary>
    public string? ContentSearchIndex { get; set; }

    /// <summary>
    /// Gets or sets the preview of the note.
    /// </summary>
    public string? Preview { get; set; }

    /// <summary>
    /// Gets or sets the book associated with the note.
    /// </summary>
    public string? Book { get; set; }

    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    public string Tags
    {
        get => tags;
        set
        {
            tags = value;
            tagsList = TagsHelper.Extract(tags);
        }
    }

    /// <summary>
    /// Gets or sets the tags list.
    /// </summary>
    [NotMapped]
    public List<string> TagsList
    {
        get => tagsList;
        set
        {
            tagsList = value ?? [];
            Tags = TagsHelper.Compress(tagsList);
        }
    }

    /// <summary>
    /// Gets or sets the list of sources associated with the note.
    /// </summary>
    public List<NotesSource> Sources { get; set; } = [];

    /// <summary>
    /// Gets or sets the full-text search entity associated with the note.
    /// </summary>
    public NoteFtsEntity NoteFts { get; set; } = null!;

    /// <summary>
    /// Sets the value of the tags field.
    /// </summary>
    /// <param name="tags">The tags string to set.</param>
    public void SetTags(string tags)
    {
        this.tags = tags;
        tagsList = TagsHelper.Extract(tags);
    }
}
