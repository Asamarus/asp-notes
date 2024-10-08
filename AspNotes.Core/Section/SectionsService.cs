﻿using AspNotes.Core.Common;
using AspNotes.Core.Section.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNotes.Core.Section;

/// <summary>
/// The SectionsService class provides methods to interact with the Sections in the database.
/// </summary>
public class SectionsService(NotesDbContext context, AppCache appCache) : ISectionsService
{
    /// <summary>
    /// Checks if a section name is unique in the database.
    /// </summary>
    /// <param name="name">The name of the section.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the section name is unique.</returns>
    public async Task<bool> IsSectionNameUnique(string name)
    {
        return !await context.Sections.AnyAsync(x => x.Name == name);
    }

    /// <summary>
    /// Checks if a section ID is present in the database.
    /// </summary>
    /// <param name="id">The ID of the section.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the section ID is present.</returns>
    public async Task<bool> IsSectionIdPresent(long id)
    {
        return await context.Sections.AnyAsync(x => x.Id == id);
    }

    /// <summary>
    /// Checks if a section name is valid.
    /// </summary>
    /// <param name="name">The name of the section.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the section name is valid.</returns>
    public async Task<bool> IsSectionNameValid(string name)
    {
        var sections = await GetSections();
        return sections.Any(x => x.Name == name);
    }

    /// <summary>
    /// Checks if a section has any notes.
    /// </summary>
    /// <param name="name">The name of the section.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the section has any notes.</returns>
    public async Task<bool> IsSectionHavingNotes(string name)
    {
        return await context.Notes.AnyAsync(x => x.Section == name);
    }

    /// <summary>
    /// Retrieves all sections from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of sections.</returns>
    public async Task<List<SectionDto>> GetSections()
    {
        var result = appCache.GetCache<List<SectionDto>>(CacheKeys.Sections);

        if (result != null)
            return result;

        var sections = await context.Sections.OrderBy(x => x.Position).AsNoTracking().ToListAsync();

        return sections.Select(s => new SectionDto
        {
            Id = s.Id,
            Name = s.Name,
            DisplayName = s.DisplayName,
            Color = s.Color,
            Position = s.Position
        }).ToList();
    }

    /// <summary>
    /// Creates a new section with the specified details and adds it to the database.
    /// </summary>
    /// <param name="name">The unique name of the section.</param>
    /// <param name="displayName">The display name of the section.</param>
    /// <param name="color">The color associated with the section.</param>
    /// <returns>The ID of the newly created section.</returns>
    /// <remarks>
    /// This method automatically assigns the next available position to the new section.
    /// It also updates the sections cache after adding the new section.
    /// </remarks>
    public async Task<long> CreateSection(string name, string displayName, string color)
    {
        var maxPosition = context.Sections.Any()
            ? await context.Sections.MaxAsync(x => x.Position)
            : 0;

        var entity = new SectionEntity
        {
            Name = name,
            DisplayName = displayName,
            Color = color,
            Position = maxPosition + 1
        };

        context.Sections.Add(entity);
        await context.SaveChangesAsync();

        appCache.RemoveCache(CacheKeys.Sections);
        appCache.UpdateCache(CacheKeys.Sections, await GetSections());

        return entity.Id;
    }

    /// <summary>
    /// Updates the details of an existing section.
    /// </summary>
    /// <param name="id">The ID of the section to update.</param>
    /// <param name="displayName">The new display name for the section.</param>
    /// <param name="color">The new color for the section.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a value of true if the update was successful.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the specified section cannot be found.</exception>
    /// <remarks>
    /// This method updates the display name and color of the specified section.
    /// It also clears and updates the sections cache after the update.
    /// </remarks>
    public async Task<bool> UpdateSection(long id, string displayName, string color)
    {
        var entity = await context.Sections.FindAsync(id) ?? throw new InvalidOperationException("Section not found.");

        entity.DisplayName = displayName;
        entity.Color = color;

        await context.SaveChangesAsync();

        appCache.RemoveCache(CacheKeys.Sections);
        appCache.UpdateCache(CacheKeys.Sections, await GetSections());

        return true;
    }

    /// <summary>
    /// Deletes a section from the database.
    /// </summary>
    /// <param name="id">The ID of the section to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value that indicates whether the operation was successful.</returns>
    public async Task<bool> DeleteSection(long id)
    {
        var entity = await context.Sections.FindAsync(id) ?? throw new InvalidOperationException("Section not found.");

        var affectedEntities = context.Sections.Where(s => s.Position > entity.Position);

        foreach (var affectedEntity in affectedEntities)
        {
            affectedEntity.Position--;
        }

        context.Sections.Remove(entity);
        await context.SaveChangesAsync();

        appCache.RemoveCache(CacheKeys.Sections);
        appCache.UpdateCache(CacheKeys.Sections, await GetSections());

        return true;
    }

    /// <summary>
    /// Reorders the sections in the database.
    /// </summary>
    /// <param name="sectionIds">The IDs of the sections in the new order.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the reordering was successful.</returns>
    public async Task<bool> ReorderSections(List<long> sectionIds)
    {
        var entities = await context.Sections
            .Where(x => sectionIds.Contains(x.Id))
            .ToListAsync();

        if (entities.Count != sectionIds.Count)
            return false;

        foreach (var entity in entities)
        {
            entity.Position = (uint)sectionIds.IndexOf(entity.Id);
        }

        context.Sections.UpdateRange(entities);
        await context.SaveChangesAsync();

        appCache.RemoveCache(CacheKeys.Sections);
        appCache.UpdateCache(CacheKeys.Sections, await GetSections());

        return true;
    }
}
