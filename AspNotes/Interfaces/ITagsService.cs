using Microsoft.EntityFrameworkCore.Storage;

namespace AspNotes.Interfaces;

/// <summary>
/// Defines the contract for a service that manages tags associated with notes.
/// </summary>
public interface ITagsService
{
    /// <summary>
    /// Updates the tags associated with a specific note.
    /// </summary>
    /// <param name="noteId">The ID of the note to update tags for.</param>
    /// <param name="newTags">A set of new tags to associate with the note.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <param name="transaction">An optional transaction to use. If null, a new transaction is created.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a boolean indicating whether the update was successful.</returns>
    Task<bool> UpdateNoteTags(
        long noteId,
        HashSet<string> newTags,
        CancellationToken cancellationToken,
        IDbContextTransaction? transaction = null);

    /// <summary>
    /// Provides tag suggestions based on a search term, optionally filtered by section.
    /// </summary>
    /// <param name="searchTerm">The term to search for in tag names.</param>
    /// <param name="section">The section to filter tags by.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a list of autocomplete suggestions.</returns>
    Task<List<(string Id, string Name)>> Autocomplete(
        string searchTerm,
        string? section,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a list of tags, optionally filtered by section, along with their usage count.
    /// </summary>
    /// <param name="section">Optional. The section to filter tags by.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a list of tags along with their usage count.</returns>
    Task<List<(long Id, string Name, int Count)>> GetTags(
        string? section,
        CancellationToken cancellationToken);
}