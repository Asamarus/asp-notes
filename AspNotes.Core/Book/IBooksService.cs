using AspNotes.Core.Book.Models;

namespace AspNotes.Core.Book;

public interface IBooksService
{
    Task<bool> UpdateNoteBook(long noteId, string book);
    Task<List<BooksServiceAutoCompleteResultItem>> Autocomplete(string searchTerm, string? section = null);
    Task<List<BooksServiceGetBooksResultItem>> GetBooks(string? section = null);
}
