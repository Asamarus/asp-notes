using AspNotes.Entities;
using AspNotes.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNotes.Tests.Services;

public class BooksServiceTests : InMemoryDatabaseTestBase
{
    private readonly BooksService booksService;
    private readonly Mock<ILogger<BooksService>> loggerMock;

    public BooksServiceTests()
    {
        loggerMock = new Mock<ILogger<BooksService>>();
        booksService = new BooksService(DbContext, loggerMock.Object);
    }

    [Fact]
    public async Task UpdateNoteBook_ShouldReturnFalse_WhenNoteDoesNotExist()
    {
        // Arrange
        var noteId = 9999;
        var bookName = "New Book";

        // Act
        var result = await booksService.UpdateNoteBook(noteId, bookName, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateNoteBook_ShouldReturnTrue_WhenBookIsSame()
    {
        // Arrange
        var note = new NoteEntity { Section = "Test", Book = "Existing Book" };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await booksService.UpdateNoteBook(note.Id, "Existing Book", CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateNoteBook_ShouldUpdateBook_WhenBookIsDifferent()
    {
        // Arrange
        var note = new NoteEntity { Section = "Test", Book = "Old Book" };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await booksService.UpdateNoteBook(note.Id, "New Book", CancellationToken.None);
        var updatedNote = await DbContext.Notes.FindAsync(note.Id);

        // Assert
        Assert.True(result);
        Assert.NotNull(updatedNote);
        Assert.Equal("New Book", updatedNote.Book);
    }

    [Fact]
    public async Task UpdateNoteBook_ShouldAddNewBook_WhenBookIsNew()
    {
        // Arrange
        var bookName = "New book";

        DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1"
        });

        await DbContext.SaveChangesAsync();

        // Act
        var updated = await booksService.UpdateNoteBook(1L, bookName, CancellationToken.None);

        // Assert
        Assert.True(updated);
        var note = await DbContext.Notes.FindAsync(1L);
        Assert.NotNull(note);
        Assert.Equal(bookName, note.Book);

        var book = await DbContext.Books.FirstOrDefaultAsync(x => x.Name == bookName && x.Section == "section1");
        Assert.NotNull(book);
        Assert.Equal(bookName, book.Name);
    }

    [Fact]
    public async Task UpdateNoteBook_ShouldUpdatesExistingBook_WhenBookExists()
    {
        // Arrange
        var existingBookName = "Existing Book";
        var section = "section1";
        await DbContext.Books.AddAsync(new BookEntity { Name = existingBookName, Section = section });
        await DbContext.SaveChangesAsync();

        DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = section
        });

        await DbContext.SaveChangesAsync();

        // Act
        var updated = await booksService.UpdateNoteBook(1L, existingBookName, CancellationToken.None);

        // Assert
        Assert.True(updated);
        var note = await DbContext.Notes.FindAsync(1L);
        Assert.NotNull(note);
        Assert.Equal(existingBookName, note.Book);

        var bookCount = await DbContext.Books.CountAsync(x => x.Name == existingBookName && x.Section == section);
        Assert.Equal(1, bookCount); // Ensure no duplicate books are created
    }

    [Fact]
    public async Task UpdateNoteBook_ShouldRemoveOldBook_WhenNoOtherNotesReferToIt()
    {
        // Arrange
        var oldBookName = "Old Book";
        var newBookName = "New Book";
        var section = "section1";
        await DbContext.Books.AddAsync(new BookEntity { Name = oldBookName, Section = section });
        await DbContext.SaveChangesAsync();

        var newNote = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = section
        };

        DbContext.Notes.Add(newNote);

        await DbContext.SaveChangesAsync();

        await booksService.UpdateNoteBook(newNote.Id, oldBookName, CancellationToken.None);

        // Act
        var updated = await booksService.UpdateNoteBook(newNote.Id, newBookName, CancellationToken.None);

        // Assert
        Assert.True(updated);
        var note = await DbContext.Notes.FindAsync(newNote.Id);
        Assert.NotNull(note);
        Assert.Equal(newBookName, note.Book);

        var oldBook = await DbContext.Books.FirstOrDefaultAsync(x => x.Name == oldBookName && x.Section == section);
        Assert.Null(oldBook); // Old book should be removed

        var newBook = await DbContext.Books.FirstOrDefaultAsync(x => x.Name == newBookName && x.Section == section);
        Assert.NotNull(newBook); // New book should be added
    }

    [Fact]
    public async Task UpdateNoteBook_ShouldKeepOldBook_WhenOtherNotesReferToIt()
    {
        // Arrange
        var oldBookName = "Old Book";
        var newBookName = "New Book";
        var section = "section1";
        await DbContext.Books.AddAsync(new BookEntity { Name = oldBookName, Section = section });
        await DbContext.SaveChangesAsync();

        var newNote = new NoteEntity
        {
            Title = "Note title 1",
            Content = "Some text 1",
            Book = oldBookName,
            Section = section
        };

        DbContext.Notes.Add(newNote);
        DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note title 2",
            Content = "Some text 2",
            Book = oldBookName,
            Section = section
        });

        await DbContext.SaveChangesAsync();

        // Act
        var updated = await booksService.UpdateNoteBook(newNote.Id, newBookName, CancellationToken.None);

        // Assert
        Assert.True(updated);
        var note1 = await DbContext.Notes.FindAsync(newNote.Id);
        Assert.NotNull(note1);
        Assert.Equal(newBookName, note1.Book);

        var oldBook = await DbContext.Books.FirstOrDefaultAsync(x => x.Name == oldBookName && x.Section == section);
        Assert.NotNull(oldBook); // Old book should be kept since another note refers to it

        var newBook = await DbContext.Books.FirstOrDefaultAsync(x => x.Name == newBookName && x.Section == section);
        Assert.NotNull(newBook); // New book should be added
    }

    [Fact]
    public async Task UpdateNoteBook_ShouldRemoveBook_WhenBookIsDeleted()
    {
        // Arrange

        var bookName = "Some book";
        var section = "section1";

        var newNote = new NoteEntity
        {
            Title = "Note title 1",
            Content = "Some text 1",
            Section = section
        };

        DbContext.Notes.Add(newNote);

        await DbContext.SaveChangesAsync();
        await booksService.UpdateNoteBook(newNote.Id, "Some book", CancellationToken.None);

        // Act
        var updated = await booksService.UpdateNoteBook(newNote.Id, string.Empty, CancellationToken.None);

        // Assert
        Assert.True(updated);
        var note1 = await DbContext.Notes.FindAsync(newNote.Id);
        Assert.NotNull(note1);
        Assert.Null(note1.Book);

        var book = await DbContext.Books.FirstOrDefaultAsync(x => x.Name == bookName && x.Section == section);
        Assert.Null(book); // Book should be removed
    }

    [Fact]
    public async Task Autocomplete_ShouldReturnMatchingBooks()
    {
        // Arrange
        var book1 = new BookEntity { Name = "Book One", Section = "Test" };
        var book2 = new BookEntity { Name = "Book Two", Section = "Test" };
        DbContext.Books.AddRange(book1, book2);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await booksService.Autocomplete("Book", "Test", CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, b => b.Name == "Book One");
        Assert.Contains(result, b => b.Name == "Book Two");
    }

    [Fact]
    public async Task Autocomplete_ShouldReturnBooksFromSpecificSection_WhenSectionIsSpecified()
    {
        // Arrange
        var searchTerm = "Book";
        var specificSection = "section1";
        await DbContext.Books.AddRangeAsync(new BookEntity
        {
            Name = "Book One",
            Section = "section1"
        }, new BookEntity
        {
            Name = "Another Book",
            Section = "section2"
        });
        await DbContext.SaveChangesAsync();

        // Act
        var results = await booksService.Autocomplete(searchTerm, specificSection, CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.All(results, r => Assert.NotEqual("Another Book", r.Name));
    }

    [Fact]
    public async Task Autocomplete_ShouldReturnEmptyList_WhenNoBooksMatch()
    {
        // Arrange
        var searchTerm = "Nonexistent";

        // Act
        var results = await booksService.Autocomplete(searchTerm, null, CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetBooks_ShouldReturnBooksWithCount()
    {
        // Arrange
        var note = new NoteEntity { Section = "Test" };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();
        await booksService.UpdateNoteBook(note.Id, "Book One", CancellationToken.None);

        // Act
        var result = await booksService.GetBooks("Test", CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, b => b.Name == "Book One" && b.Count == 1);
    }

    [Fact]
    public async Task GetBooks_ShouldReturnAllBooks_WhenSectionIsNull()
    {
        // Arrange
        await DbContext.Books.AddRangeAsync(new BookEntity
        {
            Name = "Book One",
            Section = "section1"
        }, new BookEntity
        {
            Name = "Book Two",
            Section = "section2"
        });
        await DbContext.SaveChangesAsync();

        // Act
        var results = await booksService.GetBooks(null, CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }
}
