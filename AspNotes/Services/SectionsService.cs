using AspNotes.Common;
using AspNotes.Entities;
using AspNotes.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AspNotes.Services;

/// <summary>
/// The SectionsService class provides methods to interact with the Sections in the database.
/// </summary>
public class SectionsService(IAppDbContext dbContext, IAppCache appCache)
    : ISectionsService
{
    /// <inheritdoc />
    public async Task<bool> IsSectionNameUnique(string name, CancellationToken cancellationToken)
    {
        return !await dbContext.Sections.AnyAsync(x => x.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsSectionIdPresent(long id, CancellationToken cancellationToken)
    {
        return await dbContext.Sections.AnyAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsSectionNameValid(string name, CancellationToken cancellationToken)
    {
        var sections = await GetSections(cancellationToken);
        return sections.Exists(x => x.Name == name);
    }

    /// <inheritdoc />
    public async Task<bool> IsSectionHavingNotes(long id, CancellationToken cancellationToken)
    {
        var section = await dbContext.Sections.FindAsync([id], cancellationToken);

        return section != null && await dbContext.Notes.AnyAsync(x => x.Section == section.Name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<SectionEntity>> GetSections(CancellationToken cancellationToken)
    {
        var result = appCache.GetCache<List<SectionEntity>>(CacheKeys.Sections);

        if (result != null)
        {
            return result;
        }

        var sections = await dbContext.Sections.OrderBy(x => x.Position)
            .AsNoTracking().ToListAsync(cancellationToken);

        return sections;
    }

    /// <inheritdoc />
    public async Task<long> CreateSection(string name, string displayName, string color, CancellationToken cancellationToken)
    {
        var hasSections = await dbContext.Sections.AnyAsync(cancellationToken);
        var maxPosition = hasSections
            ? await dbContext.Sections.MaxAsync(x => x.Position, cancellationToken)
            : 0;

        var entity = new SectionEntity
        {
            Name = name,
            DisplayName = displayName,
            Color = color,
            Position = maxPosition + 1,
        };

        dbContext.Sections.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        appCache.RemoveCache(CacheKeys.Sections);
        appCache.UpdateCache(CacheKeys.Sections, await GetSections(cancellationToken));

        return entity.Id;
    }

    /// <inheritdoc />
    public async Task UpdateSection(long id, string displayName, string color, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Sections.FindAsync([id], cancellationToken)
            ?? throw new InvalidOperationException("Section not found.");

        entity.DisplayName = displayName;
        entity.Color = color;

        await dbContext.SaveChangesAsync(cancellationToken);

        appCache.RemoveCache(CacheKeys.Sections);
        appCache.UpdateCache(CacheKeys.Sections, await GetSections(cancellationToken));
    }

    /// <inheritdoc />
    public async Task DeleteSection(long id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Sections.FindAsync([id], cancellationToken)
            ?? throw new InvalidOperationException("Section not found.");

        var affectedEntities = dbContext.Sections.Where(s => s.Position > entity.Position);

        foreach (var affectedEntity in affectedEntities)
        {
            affectedEntity.Position--;
        }

        dbContext.Sections.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        appCache.RemoveCache(CacheKeys.Sections);
        appCache.UpdateCache(CacheKeys.Sections, await GetSections(cancellationToken));
    }

    /// <inheritdoc />
    public async Task<bool> ReorderSections(List<long> sectionIds, CancellationToken cancellationToken)
    {
        var entities = await dbContext.Sections
            .Where(x => sectionIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (entities.Count != sectionIds.Count)
        {
            return false;
        }

        foreach (var entity in entities)
        {
            entity.Position = (uint)sectionIds.IndexOf(entity.Id);
        }

        dbContext.Sections.UpdateRange(entities);
        await dbContext.SaveChangesAsync(cancellationToken);

        appCache.RemoveCache(CacheKeys.Sections);
        appCache.UpdateCache(CacheKeys.Sections, await GetSections(cancellationToken));

        return true;
    }
}
