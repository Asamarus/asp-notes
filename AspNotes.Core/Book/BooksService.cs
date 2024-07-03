using AspNotes.Core.Book.Models;
using AspNotes.Core.Common;
using AspNotes.Core.Note.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace AspNotes.Core.Book;

/// <summary>
/// Provides services for managing books and their associations with notes.
/// </summary>
public class BooksService(NotesDbContext context, QueryFactory db, ILogger<BooksService> logger) : IBookService
{
    /// <summary>
    /// Updates the book associated with a given note within a transaction. If an exception occurs, the transaction is rolled back.
    /// </summary>
    /// <param name="noteId">The ID of the note to update.</param>
    /// <param name="book">The new book name to associate with the note. If null or whitespace, the association is removed.</param>
    /// <returns>A boolean value indicating whether the update was successful (true) or not (false).</returns>
    public async Task<bool> UpdateNoteBook(long noteId, string book)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var noteEntity = await context.Notes.FindAsync(noteId);

            if (noteEntity == null)
                return false;

            var oldBook = noteEntity.Book;

            if (oldBook == book)
                return true;

            if (string.IsNullOrWhiteSpace(book))
            {
                noteEntity.Book = null;
            }
            else
            {
                var currentBook = await context.Books.Where(b => EF.Functions.Like(b.Name, book) && b.Section == noteEntity.Section).FirstOrDefaultAsync();

                if (currentBook == null)
                {
                    currentBook = new BookEntity
                    {
                        Name = book,
                        Section = noteEntity.Section
                    };
                    context.Books.Add(currentBook);
                }

                noteEntity.Book = currentBook.Name;
            }

            await context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(oldBook))
            {
                var hasNotes = await context.Notes.AnyAsync(n => n.Book == oldBook && n.Section == noteEntity.Section);

                if (!hasNotes)
                {
                    var oldBookEntity = await context.Books.Where(b => b.Name == oldBook && b.Section == noteEntity.Section).FirstOrDefaultAsync();
                    if (oldBookEntity != null)
                    {
                        context.Books.Remove(oldBookEntity);
                        await context.SaveChangesAsync();
                    }
                }
            }

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
    /// Provides a list of book names that match the given search term for autocomplete purposes.
    /// </summary>
    /// <param name="searchTerm">The term to search for.</param>
    /// <param name="section">Optional. The section to filter the books by.</param>
    /// <returns>A list of <see cref="BooksServiceAutoCompleteResultItem"/> matching the search criteria.</returns>
    public async Task<List<BooksServiceAutoCompleteResultItem>> Autocomplete(string searchTerm, string? section = null)
    {
        var b = new BooksTable("b");

        var query = db.Query()
            .Select(b.Id, b.Name)
            .From(b.GetFormattedTableName())
            .WhereRaw($"{b.Name} LIKE ?", $"%{searchTerm}%")
            .Limit(10);

        if (!string.IsNullOrEmpty(section))
            query.Where(b.Section, section);

        var books = await query.GetAsync();

        return [.. books.Select(book => new BooksServiceAutoCompleteResultItem
        {
            Id = book.Id.ToString(),
            Name = book.Name
        }).OrderBy(book => book.Name, StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    /// Retrieves a list of books, optionally filtered by section, along with the count of notes associated with each book.
    /// </summary>
    /// <param name="section">Optional. The section to filter the books by.</param>
    /// <returns>A list of <see cref="BooksServiceGetBooksResultItem"/> representing the books and their associated notes count.</returns>
    public async Task<List<BooksServiceGetBooksResultItem>> GetBooks(string? section = null)
    {
        var b = new BooksTable("b");
        var n = new NotesTable("n");

        var query = db.Query()
            .Select(b.Id, b.Name)
            .SelectRaw($"Count({n.Id}) as Number")
            .From(b.GetFormattedTableName())
            .LeftJoin(n.GetFormattedTableName(), b.Name, n.Book)
            .GroupByRaw($"{b.Name}, {b.Id}");

        if (!string.IsNullOrEmpty(section))
        {
            query.Where(n.Section, section)
                 .Where(b.Section, section);
        }

        var books = await query.GetAsync();

        return [.. books.Select(book => new BooksServiceGetBooksResultItem
        {
            Id = book.Id,
            Name = book.Name,
            Number = book.Number
        }).OrderBy(book => book.Name, StringComparer.OrdinalIgnoreCase)];
    }
}
