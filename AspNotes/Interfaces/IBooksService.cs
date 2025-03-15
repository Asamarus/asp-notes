using Microsoft.EntityFrameworkCore.Storage;

namespace AspNotes.Interfaces;

/// <summary>
/// Provides methods for managing books and performing book-related operations.
/// </summary>
public interface IBooksService
{
    /// <summary>
    /// Updates the book associated with a given note within a transaction
    /// If an exception occurs, the transaction is rolled back.
    /// </summary>
    /// <param name="noteId">The ID of the note to update.</param>
    /// <param name="book">The new book name to associate with the note.
    /// If null or whitespace, the association is removed.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="transaction">An optional transaction to use. If null, a new transaction is created.</param>
    /// <returns>A boolean value indicating whether the update was successful (true) or not (false).</returns>
    Task<bool> UpdateNoteBook(
        long noteId,
        string book,
        CancellationToken cancellationToken,
        IDbContextTransaction? transaction = null);

    /// <summary>
    /// Provides a list of book names that match the given search term for autocomplete purposes.
    /// </summary>
    /// <param name="searchTerm">The term to search for.</param>
    /// <param name="section">The section to filter the books by.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a list of tuples with the ID and name of the matching books.</returns>
    Task<List<(long Id, string Name)>> Autocomplete(
        string searchTerm,
        string? section,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a list of books, optionally filtered by section, along with the count of notes associated with each book.
    /// </summary>
    /// <param name="section">The section to filter the books by.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a list of tuples with the ID, name, and count of the books.</returns>
    Task<List<(long Id, string Name, int Count)>> GetBooks(
        string? section,
        CancellationToken cancellationToken);
}