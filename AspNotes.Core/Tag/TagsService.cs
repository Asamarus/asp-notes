using AspNotes.Core.Common;
using AspNotes.Core.NoteTag.Models;
using AspNotes.Core.Tag.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace AspNotes.Core.Tag;

/// <summary>
/// Provides services for managing tags associated with notes.
/// </summary>
public class TagsService(NotesDbContext context, QueryFactory db, ILogger<TagsService> logger) : ITagsService
{
    /// <summary>
    /// Updates the tags associated with a specific note.
    /// </summary>
    /// <param name="noteId">The ID of the note to update tags for.</param>
    /// <param name="newTags">A set of new tags to associate with the note.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the update was successful.</returns>
    public async Task<bool> UpdateNoteTags(long noteId, HashSet<string> newTags)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var noteEntity = await context.Notes.FindAsync(noteId);

            if (noteEntity == null)
                return false;

            var sectionTags = await GetTags(noteEntity.Section);

            // Replace case sensitive tags
            newTags = newTags.Select(x =>
            {
                var match = sectionTags.FirstOrDefault(t => t.Name.Equals(x, StringComparison.OrdinalIgnoreCase))?.Name;
                return match ?? x;
            }).ToHashSet();

            // Check if the current tags and newTags are equal in content and count
            if (noteEntity.Tags.ToHashSet().SetEquals(newTags))
            {
                return true; // Exit early as there are no changes to be made
            }

            var tagsToAdd = newTags.Except(sectionTags.Select(t => t.Name), StringComparer.OrdinalIgnoreCase).ToHashSet();

            if (tagsToAdd.Count > 0)
            {
                context.Tags.AddRange(tagsToAdd.Select(tag => new TagEntity
                {
                    Name = tag,
                    Section = noteEntity.Section
                }));

                await context.SaveChangesAsync();

                var newTagsWithIds = await context.Tags
                    .Where(tag => tagsToAdd.Contains(tag.Name) && tag.Section == noteEntity.Section)
                    .Select(tag => new TagsServiceGetTagsResultItem
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        Number = 1
                    })
                    .ToListAsync();

                sectionTags.AddRange(newTagsWithIds);
            }

            var noteTagsToAdd = sectionTags
                .Where(x => newTags.Contains(x.Name) && !noteEntity.Tags.Contains(x.Name))
                .ToHashSet();

            if (noteTagsToAdd.Count > 0)
            {
                context.NotesTags.AddRange(noteTagsToAdd.Select(x => new NoteTagEntity
                {
                    NoteId = noteEntity.Id,
                    TagId = x.Id
                }));
            }

            var noteTagsToRemove = sectionTags
                .Where(x => !newTags.Contains(x.Name) && noteEntity.Tags.Contains(x.Name))
                .ToHashSet();

            if (noteTagsToRemove.Count > 0)
            {
                var nt = new NotesTagsTable("nt");

                db.Query()
                    .From(nt.GetFormattedTableName())
                    .Where(nt.NoteId, noteEntity.Id)
                    .WhereIn(nt.TagId, noteTagsToRemove.Select(x => x.Id))
                    .Delete(transaction.GetDbTransaction());

                sectionTags.Where(st => noteTagsToRemove.Any(r => r.Id == st.Id))
                            .ToList()
                            .ForEach(st => st.Number--);
            }

            var unusedTags = sectionTags
                .Where(x => x.Number == 0)
                .ToHashSet();

            if (unusedTags.Count > 0)
            {
                var t = new TagsTable("t");

                db.Query()
                    .From(t.GetFormattedTableName())
                    .WhereIn(t.Id, unusedTags.Select(x => x.Id))
                    .Delete(transaction.GetDbTransaction());
            }

            noteEntity.Tags = [.. newTags];

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred: {ErrorMessage}", ex.Message);
            await transaction.RollbackAsync();
            return false;
        }
    }

    /// <summary>
    /// Provides tag suggestions based on a search term, optionally filtered by section.
    /// </summary>
    /// <param name="searchTerm">The term to search for in tag names.</param>
    /// <param name="section">Optional. The section to filter tags by.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of autocomplete suggestions.</returns>
    public async Task<List<TagsServiceAutocompleteResultItem>> Autocomplete(string searchTerm, string? section = null)
    {
        var t = new TagsTable("t");

        var query = db.Query()
            .Select(t.Id, t.Name)
            .From(t.GetFormattedTableName())
            .WhereRaw($"{t.Name} LIKE ?", $"%{searchTerm}%")
            .Limit(10);

        if (!string.IsNullOrEmpty(section))
            query.Where(t.Section, section);

        var tags = await query.GetAsync();

        return [.. tags.Select(tag => new TagsServiceAutocompleteResultItem
        {
            Id = tag.Id.ToString(),
            Name = tag.Name
        }).OrderBy(book => book.Name, StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    /// Retrieves a list of tags, optionally filtered by section, along with their usage count.
    /// </summary>
    /// <param name="section">Optional. The section to filter tags by.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of tags along with their usage count.</returns>
    public async Task<List<TagsServiceGetTagsResultItem>> GetTags(string? section = null)
    {
        var t = new TagsTable("t");
        var nt = new NotesTagsTable("nt");

        var query = db.Query()
            .Select(t.Id, t.Name)
            .SelectRaw($"Count({nt.Id}) as Number")
            .From(t.GetFormattedTableName())
            .LeftJoin(nt.GetFormattedTableName(), t.Id, nt.TagId)
            .GroupByRaw($"{t.Name}, {t.Id}");

        if (!string.IsNullOrEmpty(section))
            query.Where(t.Section, section);

        var tags = await query.GetAsync();

        return [.. tags.Select(tag => new TagsServiceGetTagsResultItem
        {
            Id = tag.Id,
            Name = tag.Name,
            Number = tag.Number
        }).OrderBy(tag => tag.Name, StringComparer.OrdinalIgnoreCase)];
    }
}
