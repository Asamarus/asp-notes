using AspNotes.Core.Book;
using AspNotes.Core.Book.Models;
using AspNotes.Core.Note;
using AspNotes.Core.Note.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNotes.Core.Tests.Book;

public class BooksServiceTests : DatabaseTestBase
{
    private readonly BooksService _booksService;
    private readonly NotesService _notesService;

    public BooksServiceTests()
    {
        var mockLogger = new Mock<ILogger<BooksService>>();
        _booksService = new BooksService(DbFixture.DbContext, DbFixture.Db, mockLogger.Object);
        _notesService = new NotesService(DbFixture.DbContext, DbFixture.Db);
    }

    [Fact]
    public async Task UpdateNoteBook_AddsNewBook_WhenBookIsNew()
    {
        // Arrange
        var bookName = "New book";
        await _notesService.CreateNote(new NoteDto
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1"
        });

        // Act
        var updated = await _booksService.UpdateNoteBook(1L, bookName);

        // Assert
        Assert.True(updated);
        var note = await DbFixture.DbContext.Notes.FindAsync(1L);
        Assert.NotNull(note);
        Assert.Equal(bookName, note.Book);

        var book = await DbFixture.DbContext.Books.FirstOrDefaultAsync(x => x.Name == bookName && x.Section == "section1");
        Assert.NotNull(book);
        Assert.Equal(bookName, book.Name);
    }

    [Fact]
    public async Task UpdateNoteBook_UpdatesExistingBook_WhenBookExists()
    {
        // Arrange
        var existingBookName = "Existing Book";
        var section = "section1";
        await DbFixture.DbContext.Books.AddAsync(new BookEntity { Name = existingBookName, Section = section });
        await DbFixture.DbContext.SaveChangesAsync();

        await _notesService.CreateNote(new NoteDto
        {
            Title = "Note title",
            Content = "Some text",
            Section = section
        });

        // Act
        var updated = await _booksService.UpdateNoteBook(1L, existingBookName);

        // Assert
        Assert.True(updated);
        var note = await DbFixture.DbContext.Notes.FindAsync(1L);
        Assert.NotNull(note);
        Assert.Equal(existingBookName, note.Book);

        var bookCount = await DbFixture.DbContext.Books.CountAsync(x => x.Name == existingBookName && x.Section == section);
        Assert.Equal(1, bookCount); // Ensure no duplicate books are created
    }

    [Fact]
    public async Task UpdateNoteBook_RemovesOldBook_WhenNoOtherNotesReferToIt()
    {
        // Arrange
        var oldBookName = "Old Book";
        var newBookName = "New Book";
        var section = "section1";
        await DbFixture.DbContext.Books.AddAsync(new BookEntity { Name = oldBookName, Section = section });
        await DbFixture.DbContext.SaveChangesAsync();

        var newNote = await _notesService.CreateNote(new NoteDto
        {
            Title = "Note title",
            Content = "Some text",
            Book = oldBookName,
            Section = section
        });

        // Act
        var updated = await _booksService.UpdateNoteBook(newNote.Id, newBookName);

        // Assert
        Assert.True(updated);
        var note = await DbFixture.DbContext.Notes.FindAsync(newNote.Id);
        Assert.NotNull(note);
        Assert.Equal(newBookName, note.Book);

        var oldBook = await DbFixture.DbContext.Books.FirstOrDefaultAsync(x => x.Name == oldBookName && x.Section == section);
        Assert.Null(oldBook); // Old book should be removed

        var newBook = await DbFixture.DbContext.Books.FirstOrDefaultAsync(x => x.Name == newBookName && x.Section == section);
        Assert.NotNull(newBook); // New book should be added
    }

    [Fact]
    public async Task UpdateNoteBook_KeepsOldBook_WhenOtherNotesReferToIt()
    {
        // Arrange
        var oldBookName = "Old Book";
        var newBookName = "New Book";
        var section = "section1";
        await DbFixture.DbContext.Books.AddAsync(new BookEntity { Name = oldBookName, Section = section });
        await DbFixture.DbContext.SaveChangesAsync();

        var newNote = await _notesService.CreateNote(new NoteDto
        {
            Title = "Note title 1",
            Content = "Some text 1",
            Book = oldBookName,
            Section = section
        });

        await _notesService.CreateNote(new NoteDto
        {
            Title = "Note title 2",
            Content = "Some text 2",
            Book = oldBookName,
            Section = section
        });

        // Act
        var updated = await _booksService.UpdateNoteBook(newNote.Id, newBookName);

        // Assert
        Assert.True(updated);
        var note1 = await DbFixture.DbContext.Notes.FindAsync(newNote.Id);
        Assert.NotNull(note1);
        Assert.Equal(newBookName, note1.Book);

        var oldBook = await DbFixture.DbContext.Books.FirstOrDefaultAsync(x => x.Name == oldBookName && x.Section == section);
        Assert.NotNull(oldBook); // Old book should be kept since another note refers to it

        var newBook = await DbFixture.DbContext.Books.FirstOrDefaultAsync(x => x.Name == newBookName && x.Section == section);
        Assert.NotNull(newBook); // New book should be added
    }

    [Fact]
    public async Task UpdateNoteBook_ReturnsFalse_WhenNoteIdDoesNotExist()
    {
        // Arrange

        // Act
        var updated = await _booksService.UpdateNoteBook(999, "Nonexistent Book");

        // Assert
        Assert.False(updated);
    }

    [Fact]
    public async Task UpdateNoteBook_RemovesBook_WhenBookIsDeleted()
    {
        // Arrange

        var bookName = "Some book";
        var section = "section1";

        var newNote = await _notesService.CreateNote(new NoteDto
        {
            Title = "Note title 1",
            Content = "Some text 1",
            Section = section
        });
        await _booksService.UpdateNoteBook(newNote.Id, "Some book");

        // Act
        var updated = await _booksService.UpdateNoteBook(newNote.Id, string.Empty);

        // Assert
        Assert.True(updated);
        var note1 = await DbFixture.DbContext.Notes.FindAsync(newNote.Id);
        Assert.NotNull(note1);
        Assert.Null(note1.Book);

        var book = await DbFixture.DbContext.Books.FirstOrDefaultAsync(x => x.Name == bookName && x.Section == section);
        Assert.Null(book); // Book should be removed
    }

    [Fact]
    public async Task Autocomplete_ReturnsCorrectBooks_WhenSearchTermMatches()
    {
        // Arrange
        var searchTerm = "Book";
        await DbFixture.DbContext.Books.AddRangeAsync(new BookEntity
        {
            Name = "Book One",
            Section = "section1"
        }, new BookEntity
        {
            Name = "Another Book",
            Section = "section2"
        });
        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var results = await _booksService.Autocomplete(searchTerm);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.Name == "Book One");
        Assert.Contains(results, r => r.Name == "Another Book");
    }

    [Fact]
    public async Task Autocomplete_ReturnsBooksFromSpecificSection_WhenSectionIsSpecified()
    {
        // Arrange
        var searchTerm = "Book";
        var specificSection = "section1";
        await DbFixture.DbContext.Books.AddRangeAsync(new BookEntity
        {
            Name = "Book One",
            Section = "section1"
        }, new BookEntity
        {
            Name = "Another Book",
            Section = "section2"
        });
        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var results = await _booksService.Autocomplete(searchTerm, specificSection);

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.All(results, r => Assert.NotEqual("Another Book", r.Name));
    }

    [Fact]
    public async Task Autocomplete_ReturnsEmptyList_WhenNoBooksMatch()
    {
        // Arrange
        var searchTerm = "Nonexistent";

        // Act
        var results = await _booksService.Autocomplete(searchTerm);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetBooks_ReturnsAllBooks_WhenSectionIsNull()
    {
        // Arrange
        await DbFixture.DbContext.Books.AddRangeAsync(new BookEntity
        {
            Name = "Book One",
            Section = "section1"
        }, new BookEntity
        {
            Name = "Book Two",
            Section = "section2"
        });
        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var results = await _booksService.GetBooks(null);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task GetBooks_ReturnsBooksFromSpecificSection_WhenSectionIsProvided()
    {
        // Arrange
        var section = "section1";
        var note = await _notesService.CreateNote(new NoteDto
        {
            Title = "Note title",
            Content = "Some text",
            Section = section
        });
        await _booksService.UpdateNoteBook(note.Id, "Book One");

        note = await _notesService.CreateNote(new NoteDto
        {
            Title = "Note title",
            Content = "Some text",
            Section = section
        });
        await _booksService.UpdateNoteBook(note.Id, "Book One");

        note = await _notesService.CreateNote(new NoteDto
        {
            Title = "Note title",
            Content = "Some text",
            Section = section
        });
        await _booksService.UpdateNoteBook(note.Id, "Book Two");

        // Act
        var results = await _booksService.GetBooks(section);

        // Assert
        var expectedResults = new List<BooksServiceGetBooksResultItem>
        {
            new() { Id = 1, Name = "Book One", Number = 2 },
            new() { Id = 2, Name = "Book Two", Number = 1 }
        };

        results.Should().BeEquivalentTo(expectedResults);
    }
}
