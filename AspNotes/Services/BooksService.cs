using AspNotes.Entities;
using AspNotes.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AspNotes.Services;

/// <summary>
/// Provides services for managing books and their associations with notes.
/// </summary>
public class BooksService(IAppDbContext dbContext, ILogger<BooksService> logger)
    : IBooksService
{
    /// <inheritdoc />
    public async Task<bool> UpdateNoteBook(
        long noteId,
        string book,
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

            var oldBook = noteEntity.Book;

            if (oldBook == book)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(book))
            {
                noteEntity.Book = null;
            }
            else
            {
                var currentBook = await dbContext.Books
                    .Where(b => EF.Functions.Like(b.Name, book) && b.Section == noteEntity.Section)
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentBook == null)
                {
                    currentBook = new BookEntity
                    {
                        Name = book,
                        Section = noteEntity.Section,
                    };
                    dbContext.Books.Add(currentBook);
                }

                noteEntity.Book = currentBook.Name;
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(oldBook))
            {
                var hasNotes = await dbContext.Notes
                    .AnyAsync(n => n.Book == oldBook && n.Section == noteEntity.Section, cancellationToken);

                if (!hasNotes)
                {
                    var oldBookEntity = await dbContext.Books
                        .Where(b => b.Name == oldBook && b.Section == noteEntity.Section)
                        .FirstOrDefaultAsync(cancellationToken);
                    if (oldBookEntity != null)
                    {
                        dbContext.Books.Remove(oldBookEntity);
                        await dbContext.SaveChangesAsync(cancellationToken);
                    }
                }
            }

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
    public async Task<List<(long Id, string Name)>> Autocomplete(
        string searchTerm,
        string? section,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Books.AsQueryable();

        if (!string.IsNullOrEmpty(section))
        {
            query = query.Where(b => b.Section == section);
        }

        var books = await query
           .Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%"))
           .Select(b => new { b.Id, b.Name })
           .Take(10)
           .ToListAsync(cancellationToken);

        return [.. books.Select(b => (b.Id, b.Name)).OrderBy(b => b.Name, StringComparer.OrdinalIgnoreCase)];
    }

    /// <inheritdoc />
    public async Task<List<(long Id, string Name, int Count)>> GetBooks(
        string? section,
        CancellationToken cancellationToken)
    {
        var query = from b in dbContext.Books
                    join l in dbContext.Notes on b.Name equals l.Book into notesGroup
                    from l in notesGroup.DefaultIfEmpty()
                    where section == null || (l.Section == section && b.Section == section)
                    group l by new { b.Id, b.Name } into g
                    select new
                    {
                        g.Key.Id,
                        g.Key.Name,
                        Count = g.Count(n => n != null),
                    };

        var books = await query.ToListAsync(cancellationToken);

        return [.. books.Select(b => (b.Id, b.Name, b.Count)).OrderBy(b => b.Name, StringComparer.OrdinalIgnoreCase)];
    }
}