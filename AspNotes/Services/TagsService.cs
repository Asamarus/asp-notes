using AspNotes.Entities;
using AspNotes.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AspNotes.Services;

/// <summary>
/// Provides services for managing tags associated with notes.
/// </summary>
public class TagsService(IAppDbContext dbContext, ILogger<TagsService> logger)
    : ITagsService
{
    /// <inheritdoc />
    public async Task<bool> UpdateNoteTags(
        long noteId,
        HashSet<string> newTags,
        CancellationToken cancellationToken,
        IDbContextTransaction? transaction = null)
    {
        var isNewTransaction = transaction == null;

        if (isNewTransaction)
        {
            transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        try
        {
            var noteEntity = await dbContext.Notes.FindAsync([noteId], cancellationToken);

            if (noteEntity == null)
            {
                return false;
            }

            var sectionTags = await GetTags(noteEntity.Section, cancellationToken);

            // Replace case sensitive tags
            newTags = [.. newTags.Select(x =>
            {
                var match = sectionTags.Find(t => t.Name.Equals(x, StringComparison.OrdinalIgnoreCase));
                if (match != default)
                {
                    return match.Name;
                }

                return x;
            })];

            // Check if the current tags and newTags are equal in content and count
            if (noteEntity.TagsList.ToHashSet().SetEquals(newTags))
            {
                return true; // Exit early as there are no changes to be made
            }

            var tagsToAdd = newTags.Except(sectionTags.Select(t => t.Name), StringComparer.OrdinalIgnoreCase).ToHashSet();

            if (tagsToAdd.Count > 0)
            {
                dbContext.Tags.AddRange(tagsToAdd.Select(tag => new TagEntity
                {
                    Name = tag,
                    Section = noteEntity.Section,
                }));

                await dbContext.SaveChangesAsync(cancellationToken);

                var newTagsWithIds = await dbContext.Tags
                    .Where(tag => tagsToAdd.Contains(tag.Name) && tag.Section == noteEntity.Section)
                    .Select(tag => new { tag.Id, tag.Name, Count = 1 })
                    .ToListAsync(cancellationToken);

                sectionTags.AddRange(newTagsWithIds.Select(tag => (tag.Id, tag.Name, tag.Count)));
            }

            var noteTagsToAdd = sectionTags
                .Where(x => newTags.Contains(x.Name) && !noteEntity.TagsList.Contains(x.Name))
                .ToHashSet();

            if (noteTagsToAdd.Count > 0)
            {
                dbContext.NotesTags.AddRange(noteTagsToAdd.Select(x => new NoteTagEntity
                {
                    NoteId = noteEntity.Id,
                    TagId = x.Id,
                }));
            }

            var noteTagsToRemove = sectionTags
                .Where(x => !newTags.Contains(x.Name) && noteEntity.TagsList.Contains(x.Name))
                .ToHashSet();

            if (noteTagsToRemove.Count > 0)
            {
                var noteTagEntitiesToRemove = await dbContext.NotesTags
                    .Where(lt => noteTagsToRemove.Select(x => x.Id).Contains(lt.TagId) && lt.NoteId == noteEntity.Id)
                    .ToListAsync(cancellationToken);

                dbContext.NotesTags.RemoveRange(noteTagEntitiesToRemove);

                for (int i = 0; i < sectionTags.Count; i++)
                {
                    if (noteTagsToRemove.Any(r => r.Id == sectionTags[i].Id))
                    {
                        sectionTags[i] = (sectionTags[i].Id, sectionTags[i].Name, sectionTags[i].Count - 1);
                    }
                }
            }

            var unusedTags = sectionTags
                 .Where(x => x.Count == 0)
                 .ToHashSet();

            if (unusedTags.Count > 0)
            {
                var tagEntitiesToRemove = await dbContext.Tags
                    .Where(t => unusedTags.Select(x => x.Id).Contains(t.Id) && t.Section == noteEntity.Section)
                    .ToListAsync(cancellationToken);

                dbContext.Tags.RemoveRange(tagEntitiesToRemove);
            }

            noteEntity.TagsList = [.. newTags];

            await dbContext.SaveChangesAsync(cancellationToken);

            if (isNewTransaction && transaction != null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred: {ErrorMessage}", ex.Message);

            if (isNewTransaction && transaction != null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            return false;
        }
        finally
        {
            if (isNewTransaction && transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }
    }

    /// <inheritdoc />
    public async Task<List<(string Id, string Name)>> Autocomplete(
        string searchTerm,
        string? section,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Tags
            .Where(t => EF.Functions.Like(t.Name, $"%{searchTerm}%"))
            .Select(t => new { t.Id, t.Name, t.Section })
            .Take(10);

        if (!string.IsNullOrEmpty(section))
        {
            query = query.Where(t => t.Section == section);
        }

        var tags = await query.ToListAsync(cancellationToken);

        return [.. tags.Select(tag => (tag.Id.ToString(), tag.Name)).OrderBy(tag => tag.Name, StringComparer.OrdinalIgnoreCase)];
    }

    /// <inheritdoc />
    public async Task<List<(long Id, string Name, int Count)>> GetTags(
        string? section,
        CancellationToken cancellationToken)
    {
        var query = from t in dbContext.Tags
                    join lt in dbContext.NotesTags on t.Id equals lt.TagId into notesGroup
                    from lt in notesGroup.DefaultIfEmpty()
                    where section == null || t.Section == section
                    group lt by new { t.Id, t.Name } into g
                    select new
                    {
                        g.Key.Id,
                        g.Key.Name,
                        Count = g.Count(n => n != null),
                    };

        var tags = await query.ToListAsync(cancellationToken);

        return [.. tags.Select(tag => (tag.Id, tag.Name, tag.Count)).OrderBy(tag => tag.Name, StringComparer.OrdinalIgnoreCase)];
    }
}
