using AspNotes.Core.Book;
using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Note;
using AspNotes.Core.Note.Models;
using AspNotes.Core.Tag;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNotes.Core.Tests.Note;

public class NotesServiceTests : DatabaseTestBase
{
    private readonly NotesService _notesService;

    public NotesServiceTests()
    {
        _notesService = new NotesService(DbFixture.DbContext, DbFixture.Db);
    }

    [Fact]
    public async Task IsNoteIdPresent_ReturnsTrue_WhenNoteExists()
    {
        //Arrange
        var note = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1"
        };

        DbFixture.DbContext.Notes.Add(note);
        await DbFixture.DbContext.SaveChangesAsync();

        //Act
        var result = await _notesService.IsNoteIdPresent(note.Id);

        //Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsNoteIdPresent_ReturnsFalse_WhenNoteDoesNotExist()
    {
        //Arrange

        //Act
        var result = await _notesService.IsNoteIdPresent(1);

        //Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Search_ReturnsAllNotes_WithDefaultSearchRequest()
    {
        //Arrange
        var note1 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1"
        };

        DbFixture.DbContext.Notes.Add(note1);

        var note2 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section2"
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();

        //Act
        var result = await _notesService.Search(new NotesServiceSearchRequest());

        //Assert
        Assert.Equal(2, result.Notes.Count);
    }

    [Fact]
    public async Task Search_ReturnsCorrectResults_WhenSearchingBySection()
    {
        //Arrange
        var section = "section1";
        var note1 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = section
        };

        DbFixture.DbContext.Notes.Add(note1);

        var note2 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section2"
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();

        //Act
        var result = await _notesService.Search(new NotesServiceSearchRequest
        {
            Section = section
        });

        //Assert
        Assert.Single(result.Notes);
        var resultNote = result.Notes.First();
        Assert.Equal(note1.Id, resultNote.Id);
        Assert.Equal(section, resultNote.Section);
    }

    [Fact]
    public async Task Search_ReturnsCorrectResult_WhenSearchingByBook()
    {
        //Arrange
        var mockLogger = new Mock<ILogger<BooksService>>();
        var bookService = new BooksService(DbFixture.DbContext, DbFixture.Db, mockLogger.Object);
        var book = "book1";

        var note1 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1",
        };

        DbFixture.DbContext.Notes.Add(note1);

        await DbFixture.DbContext.SaveChangesAsync();
        await bookService.UpdateNoteBook(note1.Id, book);

        var note2 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section2",
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();
        await bookService.UpdateNoteBook(note2.Id, "book2");

        //Act
        var result = await _notesService.Search(new NotesServiceSearchRequest
        {
            Book = book
        });

        //Assert
        Assert.Single(result.Notes);
        var resultNote = result.Notes.First();
        Assert.Equal(note1.Id, resultNote.Id);
        Assert.Equal(book, resultNote.Book);
    }

    [Fact]
    public async Task Search_ReturnsCorrectResult_WhenSearchingByTags()
    {
        //Arrange
        var mockLogger = new Mock<ILogger<TagsService>>();
        var tagsService = new TagsService(DbFixture.DbContext, DbFixture.Db, mockLogger.Object);
        var tags = new List<string> { "tag1", "tag2" };

        var note1 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1",
        };

        DbFixture.DbContext.Notes.Add(note1);

        await DbFixture.DbContext.SaveChangesAsync();
        await tagsService.UpdateNoteTags(note1.Id, [.. tags]);

        var note2 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section2",
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();
        await tagsService.UpdateNoteTags(note2.Id, ["tag3"]);

        //Act
        var result = await _notesService.Search(new NotesServiceSearchRequest
        {
            Tags = tags
        });

        //Assert
        Assert.Single(result.Notes);
        var resultNote = result.Notes.First();
        Assert.Equal(note1.Id, resultNote.Id);
        Assert.Equal(tags, resultNote.Tags);
    }

    [Fact]
    public async Task Search_ReturnsCorrectResult_WhenSearchingBySearchTerm()
    {
        //Arrange
        var searchTerm = "search term";

        var note1 = new NoteEntity
        {
            Title = "Note title",
            Content = searchTerm,
            ContentSearchIndex = searchTerm,
            Section = "section1",
        };

        DbFixture.DbContext.Notes.Add(note1);

        var note2 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            ContentSearchIndex = "Some text",
            Section = "section2",
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();
        await _notesService.UpdateNoteTitleAndContent(note1.Id, note1.Title, note1.Content);
        await _notesService.UpdateNoteTitleAndContent(note2.Id, note2.Title, note2.Content);

        //Act
        var result = await _notesService.Search(new NotesServiceSearchRequest
        {
            SearchTerm = searchTerm
        });

        //Assert
        Assert.Single(result.Notes);
        var resultNote = result.Notes.First();
        Assert.Equal(note1.Id, resultNote.Id);
        Assert.Equal(searchTerm, resultNote.Content);
    }

    [Fact]
    public async Task Search_ReturnsCorrectResult_WhenSearchingByDateRange()
    {
        //Arrange
        var date1 = new DateOnly(2021, 1, 1);
        var date2 = new DateOnly(2021, 1, 2);

        var note1 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1",
        };

        DbFixture.DbContext.Notes.Add(note1);

        var note2 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section2",
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();

        var note1Entity = await DbFixture.DbContext.Notes.FindAsync(note1.Id);

        if (note1Entity != null)
        {
            note1Entity.CreatedAt = new DateTime(date1.Year, date1.Month, date1.Day);
        }

        var note2Entity = await DbFixture.DbContext.Notes.FindAsync(note2.Id);

        if (note2Entity != null)
        {
            note2Entity.CreatedAt = new DateTime(date2.Year, date2.Month, date2.Day);
        }

        await DbFixture.DbContext.SaveChangesAsync();

        //Act
        var result = await _notesService.Search(new NotesServiceSearchRequest
        {
            FromDate = date1,
            ToDate = date1
        });

        //Assert
        Assert.Single(result.Notes);
        var resultNote = result.Notes.First();
        Assert.Equal(note1.Id, resultNote.Id);
        Assert.Equal(new DateTime(date1.Year, date1.Month, date1.Day), resultNote.CreatedAt.Date);
    }

    [Fact]
    public async Task Search_ReturnsCorrectResult_WhenSearchWithoutTags()
    {
        //Arrange
        var mockLogger = new Mock<ILogger<TagsService>>();
        var tagsService = new TagsService(DbFixture.DbContext, DbFixture.Db, mockLogger.Object);
        var tags = new List<string> { "tag1", "tag2" };

        var note1 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1",
        };

        DbFixture.DbContext.Notes.Add(note1);

        await DbFixture.DbContext.SaveChangesAsync();
        await tagsService.UpdateNoteTags(note1.Id, [.. tags]);

        var note2 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section2",
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();

        //Act
        var result = await _notesService.Search(new NotesServiceSearchRequest
        {
            WithoutTags = true
        });

        //Assert
        Assert.Single(result.Notes);
        var resultNote = result.Notes.First();
        Assert.Equal(note2.Id, resultNote.Id);
    }

    [Fact]
    public async Task Search_ReturnsCorrectResult_WhenSearchWithoutBook()
    {
        //Arrange
        var note = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1",
        };

        DbFixture.DbContext.Notes.Add(note);

        var note2 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section2",
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();

        var mockLogger = new Mock<ILogger<BooksService>>();
        var bookService = new BooksService(DbFixture.DbContext, DbFixture.Db, mockLogger.Object);
        await bookService.UpdateNoteBook(note.Id, "book1");

        //Act
        var result = await _notesService.Search(new NotesServiceSearchRequest
        {
            WithoutBook = true
        });

        //Assert
        Assert.Single(result.Notes);
        var resultNote = result.Notes.First();
        Assert.Equal(note2.Id, resultNote.Id);
    }

    [Fact]
    public async Task Search_ReturnsNotesInRandomOrder_WhenInRandomOrderIsTrue()
    {
        // Arrange
        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 1",
            Content = "Content 1",
            Section = "section1"
        });
        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 2",
            Content = "Content 2",
            Section = "section2"
        });
        await DbFixture.DbContext.SaveChangesAsync();

        var request = new NotesServiceSearchRequest { InRandomOrder = true };

        // Act
        var result1 = await _notesService.Search(request);

        // Assert
        Assert.Contains(DbFixture.SqlResults, query => query.ToString().Contains("RANDOM()"));
    }

    [Fact]
    public async Task Search_ReturnsCorrectPaginationResults_WhenPageAndResultsPerPageAreSpecified()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            DbFixture.DbContext.Notes.Add(new NoteEntity
            {
                Title = $"Note {i}",
                Content = $"Content {i}",
                Section = $"section{i % 2}"
            });
        }

        await DbFixture.DbContext.SaveChangesAsync();

        var request = new NotesServiceSearchRequest { Page = 2, ResultsPerPage = 5 };

        // Act
        var result = await _notesService.Search(request);

        // Assert
        Assert.Equal(5, result.Notes.Count); // Ensure only 5 results are returned
        Assert.Equal(2, result.Page); // Ensure the correct page is returned
        Assert.Equal(10, result.Total); // Ensure the total count is correct
        Assert.True(result.Notes.All(n => n.Title != null && n.Title.StartsWith("Note")));
    }

    [Fact]
    public async Task Autocomplete_ReturnsEmptyList_WhenNoNotesMatchSearchTerm()
    {
        // Arrange
        var searchTerm = "nonexistent";

        // Act
        var result = await _notesService.Autocomplete(searchTerm);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Autocomplete_ReturnsListOfSuggestions_WhenNotesMatchSearchTerm()
    {
        // Arrange
        var searchTerm = "Note";

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 1",
            TitleSearchIndex = SearchHelper.GetSearchIndex("Note 1"),
            Content = "Content 1",
            Section = "section1"
        });

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 2",
            TitleSearchIndex = SearchHelper.GetSearchIndex("Note 2"),
            Content = "Content 2",
            Section = "section2"
        });

        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _notesService.Autocomplete(searchTerm);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Title == "Note 1");
        Assert.Contains(result, r => r.Title == "Note 2");
    }

    [Fact]
    public async Task Autocomplete_ReturnsSuggestionsFilteredBySection_WhenSectionIsSpecified()
    {
        // Arrange
        var searchTerm = "Note";
        var section = "section1";

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 1",
            TitleSearchIndex = SearchHelper.GetSearchIndex("Note 1"),
            Content = "Content 1",
            Section = section
        });

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 2",
            TitleSearchIndex = SearchHelper.GetSearchIndex("Note 2"),
            Content = "Content 2",
            Section = "section2"
        });

        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _notesService.Autocomplete(searchTerm, section);

        // Assert
        Assert.Single(result);
        Assert.Equal("Note 1", result.First().Title);
    }

    [Fact]
    public async Task Autocomplete_ReturnsSuggestionsFilteredByBook_WhenBookIsSpecified()
    {
        // Arrange
        var searchTerm = "Note";
        var book = "book1";

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 1",
            TitleSearchIndex = SearchHelper.GetSearchIndex("Note 1"),
            Content = "Content 1",
            Book = book,
            Section = "section1"
        });

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 2",
            TitleSearchIndex = SearchHelper.GetSearchIndex("Note 2"),
            Content = "Content 2",
            Book = "book2",
            Section = "section1"
        });

        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _notesService.Autocomplete(searchTerm, book: book);

        // Assert
        Assert.Single(result);
        Assert.Equal("Note 1", result.First().Title);
    }

    [Fact]
    public async Task Autocomplete_ReturnsSuggestionsFilteredBySectionAndBook_WhenBothAreSpecified()
    {
        // Arrange
        var searchTerm = "Note";
        var section = "section1";
        var book = "book1";

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 1",
            TitleSearchIndex = SearchHelper.GetSearchIndex("Note 1"),
            Content = "Content 1",
            Section = section,
            Book = book
        });

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 2",
            TitleSearchIndex = SearchHelper.GetSearchIndex("Note 2"),
            Content = "Content 2",
            Section = section,
            Book = "book2"
        });

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "Note 3",
            TitleSearchIndex = SearchHelper.GetSearchIndex("Note 3"),
            Content = "Content 3",
            Section = "section2",
            Book = book
        });

        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _notesService.Autocomplete(searchTerm, section, book);

        // Assert
        Assert.Single(result);
        Assert.Equal("Note 1", result.First().Title);
    }

    [Fact]
    public async Task Autocomplete_LimitsResultsToTen_WhenMoreThanTenNotesMatchSearchTerm()
    {
        // Arrange
        var searchTerm = "Note";
        for (int i = 0; i < 15; i++)
        {
            DbFixture.DbContext.Notes.Add(new NoteEntity
            {
                Title = $"Note {i}",
                TitleSearchIndex = SearchHelper.GetSearchIndex($"Note {i}"),
                Content = $"Content {i}",
                Section = $"section{i % 2}"
            });
        }

        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _notesService.Autocomplete(searchTerm);

        // Assert
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Autocomplete_SortsResultsAlphabetically_WhenMultipleNotesMatchSearchTerm()
    {
        // Arrange
        var searchTerm = "Note";

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "B Note",
            TitleSearchIndex = SearchHelper.GetSearchIndex("B Note"),
            Content = "Content 2",
            Section = "section2"
        });

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Title = "A Note",
            TitleSearchIndex = SearchHelper.GetSearchIndex("A Note"),
            Content = "Content 1",
            Section = "section1"
        });

        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _notesService.Autocomplete(searchTerm);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("A Note", result.First().Title);
        Assert.Equal("B Note", result.Last().Title);
    }

    [Fact]
    public async Task GetCalendarDays_ReturnsCorrectNumberOfDays_WhenNotesExistInMultipleDays()
    {
        // Arrange
        var date1 = new DateOnly(2023, 4, 1);
        var date2 = new DateOnly(2023, 4, 2);
        var date3 = new DateOnly(2023, 4, 3);

        var note1 = new NoteEntity
        {
            Title = "Note 1",
            Content = "Content for note 1",
            Section = "section1",
        };

        DbFixture.DbContext.Notes.Add(note1);

        var note2 = new NoteEntity
        {
            Title = "Note 2",
            Content = "Content for note 2",
            Section = "section2",
        };

        DbFixture.DbContext.Notes.Add(note2);

        var note3 = new NoteEntity
        {
            Title = "Note 3",
            Content = "Content for note 3",
            Section = "section3",
        };

        DbFixture.DbContext.Notes.Add(note3);

        await DbFixture.DbContext.SaveChangesAsync();

        var note1Entity = await DbFixture.DbContext.Notes.FindAsync(note1.Id);
        if (note1Entity != null)
        {
            note1Entity.CreatedAt = date1.ToDateTime(TimeOnly.MinValue);
        }

        var note2Entity = await DbFixture.DbContext.Notes.FindAsync(note2.Id);
        if (note2Entity != null)
        {
            note2Entity.CreatedAt = date2.ToDateTime(TimeOnly.MinValue);
        }

        var note3Entity = await DbFixture.DbContext.Notes.FindAsync(note3.Id);
        if (note3Entity != null)
        {
            note3Entity.CreatedAt = date3.ToDateTime(TimeOnly.MinValue);
        }

        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _notesService.GetCalendarDays(4, 2023);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, day => day.Date == date1);
        Assert.Contains(result, day => day.Date == date2);
        Assert.Contains(result, day => day.Date == date3);
    }

    [Fact]
    public async Task GetCalendarDays_ReturnsEmptyList_WhenNoNotesExistInDateRange()
    {
        // Arrange

        // Act
        var result = await _notesService.GetCalendarDays(4, 2023);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCalendarDays_ReturnsCorrectDaysAndCounts_WhenFilteredBySection()
    {
        // Arrange
        var date1 = new DateOnly(2023, 4, 1);
        var date2 = new DateOnly(2023, 4, 2);
        var section = "section1";

        var note1 = new NoteEntity
        {
            Title = "Note 1",
            Content = "Content for note 1",
            Section = section,
        };

        DbFixture.DbContext.Notes.Add(note1);

        var note2 = new NoteEntity
        {
            Title = "Note 2",
            Content = "Content for note 2",
            Section = section,
        };

        DbFixture.DbContext.Notes.Add(note2);

        var note3 = new NoteEntity
        {
            Title = "Note 3",
            Content = "Content for note 3",
            Section = "section2",
        };

        DbFixture.DbContext.Notes.Add(note3);

        await DbFixture.DbContext.SaveChangesAsync();

        var note1Entity = await DbFixture.DbContext.Notes.FindAsync(note1.Id);
        if (note1Entity != null)
        {
            note1Entity.CreatedAt = date1.ToDateTime(TimeOnly.MinValue);
        }

        var note2Entity = await DbFixture.DbContext.Notes.FindAsync(note2.Id);
        if (note2Entity != null)
        {
            note2Entity.CreatedAt = date1.ToDateTime(TimeOnly.MinValue);
        }

        var note3Entity = await DbFixture.DbContext.Notes.FindAsync(note3.Id);
        if (note3Entity != null)
        {
            note3Entity.CreatedAt = date2.ToDateTime(TimeOnly.MinValue);
        }

        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _notesService.GetCalendarDays(4, 2023, section);

        // Assert
        Assert.Single(result);
        Assert.Contains(result, day => day.Date == date1 && day.Count == 2);
    }

    [Fact]
    public async Task GetNoteById_ReturnsNote_WhenNoteExists()
    {
        // Arrange
        var expectedNote = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "Test Section"
        };

        DbFixture.DbContext.Notes.Add(expectedNote);
        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var actualNote = await _notesService.GetNoteById(expectedNote.Id);

        // Assert
        Assert.NotNull(actualNote);
        Assert.Equal(expectedNote.Id, actualNote.Id);
        Assert.Equal("Test Note", actualNote.Title);
    }

    [Fact]
    public async Task GetNoteById_ReturnsNull_WhenNoteDoesNotExist()
    {
        // Arrange

        // Act
        var note = await _notesService.GetNoteById(1);

        // Assert
        Assert.Null(note);
    }

    [Fact]
    public async Task CreateNote_SavesNote_WithCorrectDetails()
    {
        // Arrange
        var section = "section1";

        // Act
        var savedNote = await _notesService.CreateNote(section);

        // Assert
        Assert.NotNull(savedNote);
        Assert.Equal(1, savedNote.Id);
        Assert.Equal(section, savedNote.Section);
        Assert.NotEqual(DateTime.MinValue, savedNote.CreatedAt);
        Assert.NotEqual(DateTime.MinValue, savedNote.UpdatedAt);
    }

    [Fact]
    public async Task UpdateNoteTitleAndContent_UpdatesExistingNote_WithNewDetails()
    {
        // Arrange
        var originalNote = new NoteEntity
        {
            Title = "Original Title",
            Content = "Original Content",
            Section = "Original Section"
        };

        DbFixture.DbContext.Notes.Add(originalNote);
        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var resultNote = await _notesService.UpdateNoteTitleAndContent(originalNote.Id, "Updated Title", "Updated Content");

        // Assert
        Assert.NotNull(resultNote);
        Assert.Equal(originalNote.Id, resultNote.Id);
        Assert.Equal("Updated Title", resultNote.Title);
        Assert.Equal("Updated Content", resultNote.Content);

        var rawSavedNote = await DbFixture.DbContext.Notes.FindAsync(resultNote.Id);

        Assert.NotNull(rawSavedNote);
        Assert.Equal("updated title", rawSavedNote.TitleSearchIndex);
        Assert.Equal("updated content", rawSavedNote.ContentSearchIndex);
        Assert.Equal("Updated Content", rawSavedNote.Preview);
    }

    [Fact]
    public async Task UpdateNoteTitleAndContent_DoesNotUpdateSearchIndex_IfTitleAndContentAreUnchanged()
    {
        // Arrange
        var title = "Note Title";
        var content = "Note Content";
        var originalNote = new NoteEntity
        {
            Section = "Original Section"
        };

        DbFixture.DbContext.Notes.Add(originalNote);
        await DbFixture.DbContext.SaveChangesAsync();
        await _notesService.UpdateNoteTitleAndContent(originalNote.Id, title, content);

        // Act
        await _notesService.UpdateNoteTitleAndContent(originalNote.Id, title, content);

        // Assert
        var updatedNote = await DbFixture.DbContext.Notes.FindAsync(originalNote.Id);
        Assert.NotNull(updatedNote);

        Assert.Equal(originalNote.TitleSearchIndex, updatedNote.TitleSearchIndex);
        Assert.Equal(originalNote.ContentSearchIndex, updatedNote.ContentSearchIndex);
        Assert.Equal(originalNote.Preview, updatedNote.Preview);
    }

    [Fact]
    public async Task UpdateNoteSection_UpdatesExistingNoteSection()
    {
        // Arrange
        var originalNote = new NoteEntity
        {
            Section = "Original Section"
        };

        DbFixture.DbContext.Notes.Add(originalNote);
        await DbFixture.DbContext.SaveChangesAsync();

        var newSection = "Updated Section";

        // Act
        var resultNote = await _notesService.UpdateNoteSection(originalNote.Id, newSection);

        // Assert
        Assert.NotNull(resultNote);
        Assert.Equal(originalNote.Id, resultNote.Id);
        Assert.Equal(newSection, resultNote.Section);

        var rawSavedNote = await DbFixture.DbContext.Notes.FindAsync(resultNote.Id);
        Assert.NotNull(rawSavedNote);
        Assert.Equal(newSection, rawSavedNote.Section);
    }

    [Fact]
    public async Task UpdateNoteSources_UpdatesExistingNoteSources()
    {
        // Arrange
        var originalNote = new NoteEntity
        {
            Section = "Original Section",
            Sources =
            [
                new NoteSource {
                    Id = "1",
                    Link = "http://original.com",
                    Title = "Original Source"
                }
            ]
        };

        DbFixture.DbContext.Notes.Add(originalNote);
        await DbFixture.DbContext.SaveChangesAsync();

        var updatedSources = new List<NoteSource>
        {
            new() {
                Id = "2",
                Link = "http://updated.com",
                Title = "Updated Source"
            }
        };

        // Act
        var resultNote = await _notesService.UpdateNoteSources(originalNote.Id, updatedSources);

        // Assert
        Assert.NotNull(resultNote);
        Assert.Equal(originalNote.Id, resultNote.Id);
        Assert.NotEmpty(resultNote.Sources);
        Assert.Single(resultNote.Sources);
        Assert.Equal("http://updated.com", resultNote.Sources.First().Link);
        Assert.Equal("Updated Source", resultNote.Sources.First().Title);

        var rawSavedNote = await DbFixture.DbContext.Notes.FindAsync(resultNote.Id);
        Assert.NotNull(rawSavedNote);
        Assert.NotEmpty(rawSavedNote.Sources);
        Assert.Single(rawSavedNote.Sources);
        Assert.Equal("http://updated.com", rawSavedNote.Sources.First().Link);
        Assert.Equal("Updated Source", rawSavedNote.Sources.First().Title);
    }

    [Fact]
    public async Task DeleteNote_RemovesNote_WhenNoteExists()
    {
        // Arrange
        var noteToDelete = new NoteEntity
        {
            Title = "Note to Delete",
            Content = "Content to Delete",
            Section = "Section to Delete"
        };

        DbFixture.DbContext.Notes.Add(noteToDelete);
        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var deleteResult = await _notesService.DeleteNote(noteToDelete.Id);
        var resultAfterDeletion = await _notesService.GetNoteById(noteToDelete.Id);

        // Assert
        Assert.True(deleteResult);
        Assert.Null(resultAfterDeletion);
    }

    [Fact]
    public async Task DeleteNote_ThrowsInvalidOperationException_WithExpectedMessage_WhenNoteDoesNotExist()
    {
        // Arrange
        var nonExistingId = 1; // Example ID that does not exist
        var expectedMessage = "Note not found.";

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _notesService.DeleteNote(nonExistingId));

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }
}
