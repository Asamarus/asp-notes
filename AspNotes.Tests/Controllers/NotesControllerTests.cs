using AspNotes.Controllers;
using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNotes.Tests.Controllers;

public class NotesControllerTests : InMemoryDatabaseTestBase
{
    private readonly Mock<ISectionsService> mockSectionsService;
    private readonly Mock<IBooksService> mockBooksService;
    private readonly Mock<ITagsService> mockTagsService;
    private readonly Mock<ILogger<NotesController>> mockLogger;
    private readonly NotesController controller;
    private readonly CancellationToken cancellationToken = CancellationToken.None;

    public NotesControllerTests()
    {
        mockSectionsService = new Mock<ISectionsService>();
        mockBooksService = new Mock<IBooksService>();
        mockTagsService = new Mock<ITagsService>();
        mockLogger = new Mock<ILogger<NotesController>>();
        controller = new NotesController(
            DbContext,
            mockSectionsService.Object,
            mockBooksService.Object,
            mockTagsService.Object,
            mockLogger.Object);
    }

    #region Get Tests

    [Fact]
    public async Task Get_ReturnsValidationProblem_WhenSectionIsNotValid()
    {
        // Arrange
        var request = new NotesGetRequest { Section = "InvalidSection", Page = 1 };
        mockSectionsService.Setup(s => s.IsSectionNameValid(request.Section, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await controller.Get(request, cancellationToken);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var validationProblem = Assert.IsType<ValidationProblemDetails>(objectResult.Value);
        Assert.Equal($"Section '{request.Section}' is not valid!", validationProblem.Title);
    }

    [Fact]
    public async Task Get_ReturnsOkWithPaginatedResponse_WhenRequestIsValid()
    {
        // Arrange
        var request = new NotesGetRequest { Section = "ValidSection", Page = 1 };
        mockSectionsService.Setup(s => s.IsSectionNameValid(request.Section, cancellationToken))
            .ReturnsAsync(true);

        var notes = new List<NoteEntity>
        {
            new() { Id = 1, Section = "ValidSection", Title = "Note 1", Content = "Content 1", CreatedAt = DateTime.Now.AddDays(-1) },
            new() { Id = 2, Section = "ValidSection", Title = "Note 2", Content = "Content 2", CreatedAt = DateTime.Now.AddDays(-2) }
        };

        foreach (var note in notes)
        {
            DbContext.Notes.Add(note);
        }
        await DbContext.SaveChangesAsync(cancellationToken);

        // Act
        var result = await controller.Get(request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PaginatedResponse<NotesItemResponse>>(okResult.Value);
        Assert.Equal(2, response.Total);
        Assert.Equal(2, response.Count);
        Assert.Equal(1, response.Page);
        Assert.Equal(1, response.LastPage);
        Assert.False(response.CanLoadMore);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public async Task Get_FiltersNotesByBook_WhenBookIsProvided()
    {
        // Arrange
        var bookName = "TestBook";
        var request = new NotesGetRequest { Book = bookName, Page = 1 };

        var notes = new List<NoteEntity>
        {
            new() { Id = 1, Section = "Section", Title = "Note 1", Book = bookName, Content = "Content 1", CreatedAt = DateTime.Now.AddDays(-1) },
            new() { Id = 2, Section = "Section", Title = "Note 2", Book = "OtherBook", Content = "Content 2", CreatedAt = DateTime.Now.AddDays(-2) }
        };

        foreach (var note in notes)
        {
            DbContext.Notes.Add(note);
        }
        await DbContext.SaveChangesAsync(cancellationToken);

        // Act
        var result = await controller.Get(request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PaginatedResponse<NotesItemResponse>>(okResult.Value);
        Assert.Equal(1, response.Total);
        Assert.Single(response.Data);
        Assert.Equal(bookName, response.Data.First().Book);
    }

    [Fact]
    public async Task Get_FiltersNotesByTags_WhenTagsAreProvided()
    {
        // Arrange
        var testTag = "testtag";
        var request = new NotesGetRequest { Tags = [testTag], Page = 1 };

        var notes = new List<NoteEntity>
        {
            new() { Id = 1, Section = "Section", Title = "Note 1", Content = "Content 1", CreatedAt = DateTime.Now.AddDays(-1) },
            new() { Id = 2, Section = "Section", Title = "Note 2", Content = "Content 2", CreatedAt = DateTime.Now.AddDays(-2) }
        };

        notes[0].SetTags($"[{testTag}]");
        notes[1].SetTags("[othertag]");

        foreach (var note in notes)
        {
            DbContext.Notes.Add(note);
        }
        await DbContext.SaveChangesAsync(cancellationToken);

        // Act
        var result = await controller.Get(request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PaginatedResponse<NotesItemResponse>>(okResult.Value);
        Assert.Equal(1, response.Total);
        Assert.Single(response.Data);
        Assert.Contains(testTag, response.Data.First().Tags);
    }

    [Fact]
    public async Task Get_FiltersNotesByDateRange_WhenDatesAreProvided()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Now);
        var request = new NotesGetRequest
        {
            FromDate = today.AddDays(-2),
            ToDate = today.AddDays(-1),
            Page = 1
        };

        // Use direct SQL to insert notes with specific timestamps
        var date1 = DateTime.Now.AddDays(-1.5); // This should match the filter
        var date2 = DateTime.Now.AddDays(-3);   // Too old for filter
        var date3 = DateTime.Now;               // Too new for filter

        await DbContext.Database.ExecuteSqlRawAsync(
            @"INSERT INTO Notes (
            Id, Section, Title, Content, CreatedAt, UpdatedAt, 
            Tags, Sources, TitleSearchIndex, ContentSearchIndex, Preview, Book
        ) VALUES (
            1, 'Section', 'Note 1', 'Content 1', {0}, {0}, 
            '', '[]', 'Note 1', 'Content 1', NULL, NULL
        )",
            date1);

        await DbContext.Database.ExecuteSqlRawAsync(
            @"INSERT INTO Notes (
            Id, Section, Title, Content, CreatedAt, UpdatedAt, 
            Tags, Sources, TitleSearchIndex, ContentSearchIndex, Preview, Book
        ) VALUES (
            2, 'Section', 'Note 2', 'Content 2', {0}, {0}, 
            '', '[]', 'Note 2', 'Content 2', NULL, NULL
        )",
            date2);

        await DbContext.Database.ExecuteSqlRawAsync(
            @"INSERT INTO Notes (
            Id, Section, Title, Content, CreatedAt, UpdatedAt, 
            Tags, Sources, TitleSearchIndex, ContentSearchIndex, Preview, Book
        ) VALUES (
            3, 'Section', 'Note 3', 'Content 3', {0}, {0}, 
            '', '[]', 'Note 3', 'Content 3', NULL, NULL
        )",
            date3);

        // Act
        var result = await controller.Get(request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PaginatedResponse<NotesItemResponse>>(okResult.Value);
        Assert.Equal(1, response.Total);
        Assert.Single(response.Data);
        Assert.Equal("Note 1", response.Data.First().Title);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        long nonExistentId = 999;

        // Act
        var result = await controller.Get(nonExistentId, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenNoteExists()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Section = "TestSection",
            Title = "Test Title",
            Content = "Test Content"
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        // Act
        var result = await controller.Get(noteEntity.Id, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<NotesItemResponse>(okResult.Value);
        Assert.Equal(noteEntity.Id, response.Id);
        Assert.Equal(noteEntity.Title, response.Title);
        Assert.Equal(noteEntity.Content, response.Content);
        Assert.Equal(noteEntity.Section, response.Section);
    }

    #endregion

    #region Post Tests

    [Fact]
    public async Task Post_ReturnsValidationProblem_WhenSectionIsNotValid()
    {
        // Arrange
        var request = new NotesCreateRequest { Section = "InvalidSection" };
        mockSectionsService.Setup(s => s.IsSectionNameValid(request.Section, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await controller.Post(request, cancellationToken);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var validationProblem = Assert.IsType<ValidationProblemDetails>(objectResult.Value);
        Assert.Equal($"Section '{request.Section}' is not valid!", validationProblem.Title);
    }

    [Fact]
    public async Task Post_ReturnsProblem_WhenBookUpdateFails()
    {
        // Arrange
        var request = new NotesCreateRequest { Section = "ValidSection", Book = "TestBook" };
        mockSectionsService.Setup(s => s.IsSectionNameValid(request.Section, cancellationToken))
            .ReturnsAsync(true);
        mockBooksService.Setup(b => b.UpdateNoteBook(It.IsAny<long>(), request.Book, cancellationToken, It.IsAny<IDbContextTransaction>()))
            .ReturnsAsync(false);

        // Act
        var result = await controller.Post(request, cancellationToken);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("Book update failed!", problemDetails.Title);
    }

    [Fact]
    public async Task Post_ReturnsCreatedResult_WhenSuccessful()
    {
        // Arrange
        var request = new NotesCreateRequest { Section = "ValidSection" };
        mockSectionsService.Setup(s => s.IsSectionNameValid(request.Section, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await controller.Post(request, cancellationToken);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var response = Assert.IsType<NotesItemResponse>(createdResult.Value);
        Assert.Equal(request.Section, response.Section);

        // Verify note was added to the database
        var note = await DbContext.Notes.FindAsync(response.Id);
        Assert.NotNull(note);
        Assert.Equal(request.Section, note.Section);
    }

    #endregion

    #region Patch Tests

    [Fact]
    public async Task Patch_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        long nonExistentId = 999;
        var request = new NotesPatchRequest { Title = "Updated Title" };

        // Act
        var result = await controller.Patch(nonExistentId, request, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Patch_ReturnsValidationProblem_WhenSectionIsNotValid()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Section = "TestSection",
            Title = "Test Title",
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var request = new NotesPatchRequest { Section = "InvalidSection" };
        mockSectionsService.Setup(s => s.IsSectionNameValid(request.Section, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await controller.Patch(noteEntity.Id, request, cancellationToken);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var validationProblem = Assert.IsType<ValidationProblemDetails>(objectResult.Value);
        Assert.Equal($"Section '{request.Section}' is not valid!", validationProblem.Title);
    }

    [Fact]
    public async Task Patch_UpdatesTitle_WhenTitleIsProvided()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Section = "TestSection",
            Title = "Original Title",
            Content = "Original Content"
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var newTitle = "Updated Title";
        var request = new NotesPatchRequest { Title = newTitle };

        // Act
        var result = await controller.Patch(noteEntity.Id, request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<NotesItemResponse>(okResult.Value);
        Assert.Equal(newTitle, response.Title);

        // Verify title was updated in the database
        var updatedNote = await DbContext.Notes.FindAsync(noteEntity.Id);
        Assert.NotNull(updatedNote);
        Assert.Equal(newTitle, updatedNote.Title);
    }

    [Fact]
    public async Task Patch_UpdatesContent_WhenContentIsProvided()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Section = "TestSection",
            Title = "Test Title",
            Content = "Original Content"
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var newContent = "Updated Content";
        var request = new NotesPatchRequest { Content = newContent };

        // Act
        var result = await controller.Patch(noteEntity.Id, request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<NotesItemResponse>(okResult.Value);
        Assert.Equal(newContent, response.Content);

        // Verify content was updated in the database
        var updatedNote = await DbContext.Notes.FindAsync(noteEntity.Id);
        Assert.NotNull(updatedNote);
        Assert.Equal(newContent, updatedNote.Content);
        Assert.NotNull(updatedNote.ContentSearchIndex);
        Assert.NotNull(updatedNote.Preview);
    }

    [Fact]
    public async Task Patch_UpdatesBook_WhenBookIsProvided()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Section = "TestSection",
            Title = "Test Title",
            Book = "Original Book"
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var newBook = "Updated Book";
        var request = new NotesPatchRequest { Book = newBook };

        mockBooksService.Setup(b => b.UpdateNoteBook(noteEntity.Id, newBook, cancellationToken, It.IsAny<IDbContextTransaction>()))
            .ReturnsAsync(true);

        // Act
        var result = await controller.Patch(noteEntity.Id, request, cancellationToken);

        // Assert
        Assert.IsType<OkObjectResult>(result);

        // Verify book service was called
        mockBooksService.Verify(b => b.UpdateNoteBook(noteEntity.Id, newBook, cancellationToken, It.IsAny<IDbContextTransaction>()), Times.Once);
    }

    [Fact]
    public async Task Patch_UpdatesTags_WhenTagsAreProvided()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Section = "TestSection",
            Title = "Test Title",
        };
        noteEntity.SetTags("[original]");

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var newTags = new List<string> { "updated", "tags" };
        var request = new NotesPatchRequest { Tags = newTags };

        mockTagsService.Setup(t => t.UpdateNoteTags(noteEntity.Id, It.IsAny<HashSet<string>>(), cancellationToken, It.IsAny<IDbContextTransaction>()))
            .ReturnsAsync(true);

        // Act
        var result = await controller.Patch(noteEntity.Id, request, cancellationToken);

        // Assert
        Assert.IsType<OkObjectResult>(result);

        // Verify tags service was called
        mockTagsService.Verify(t => t.UpdateNoteTags(
            noteEntity.Id,
            It.Is<HashSet<string>>(tags => tags.SetEquals(newTags)),
            cancellationToken,
            It.IsAny<IDbContextTransaction>()),
            Times.Once);
    }

    [Fact]
    public async Task Patch_UpdatesSection_WhenSectionIsProvided()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Section = "OldSection",
            Title = "Test Title",
            Book = "TestBook"
        };
        noteEntity.SetTags("[tag1][tag2]");

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var newSection = "NewSection";
        var request = new NotesPatchRequest { Section = newSection };

        mockSectionsService.Setup(s => s.IsSectionNameValid(newSection, cancellationToken))
            .ReturnsAsync(true);
        mockBooksService.Setup(b => b.UpdateNoteBook(noteEntity.Id, It.IsAny<string>(), cancellationToken, It.IsAny<IDbContextTransaction>()))
            .ReturnsAsync(true);
        mockTagsService.Setup(t => t.UpdateNoteTags(noteEntity.Id, It.IsAny<HashSet<string>>(), cancellationToken, It.IsAny<IDbContextTransaction>()))
            .ReturnsAsync(true);

        // Act
        var result = await controller.Patch(noteEntity.Id, request, cancellationToken);

        // Assert
        Assert.IsType<OkObjectResult>(result);

        // Verify section was updated in the database
        var updatedNote = await DbContext.Notes.FindAsync(noteEntity.Id);
        Assert.NotNull(updatedNote);
        Assert.Equal(newSection, updatedNote.Section);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        long nonExistentId = 999;

        // Act
        var result = await controller.Delete(nonExistentId, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsProblem_WhenBookUpdateFails()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Section = "TestSection",
            Title = "Test Title",
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        mockBooksService.Setup(b => b.UpdateNoteBook(noteEntity.Id, string.Empty, cancellationToken, It.IsAny<IDbContextTransaction>()))
            .ReturnsAsync(false);

        // Act
        var result = await controller.Delete(noteEntity.Id, cancellationToken);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("Book update failed!", problemDetails.Title);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Section = "TestSection",
            Title = "Test Title",
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        mockBooksService.Setup(b => b.UpdateNoteBook(noteEntity.Id, string.Empty, cancellationToken, It.IsAny<IDbContextTransaction>()))
            .ReturnsAsync(true);
        mockTagsService.Setup(t => t.UpdateNoteTags(noteEntity.Id, It.IsAny<HashSet<string>>(), cancellationToken, It.IsAny<IDbContextTransaction>()))
            .ReturnsAsync(true);

        // Act
        var result = await controller.Delete(noteEntity.Id, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);

        // Verify note was removed from the database
        var deletedNote = await DbContext.Notes.FindAsync(noteEntity.Id);
        Assert.Null(deletedNote);
    }

    #endregion

    #region Autocomplete Tests

    [Fact]
    public async Task GetAutocomplete_ReturnsValidationProblem_WhenSectionIsNotValid()
    {
        // Arrange
        var request = new NotesGetAutocompleteRequest
        {
            SearchTerm = "test",
            Section = "InvalidSection"
        };

        mockSectionsService.Setup(s => s.IsSectionNameValid(request.Section, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await controller.GetAutocomplete(request, cancellationToken);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var validationProblem = Assert.IsType<ValidationProblemDetails>(objectResult.Value);
        Assert.Equal($"Section '{request.Section}' is not valid!", validationProblem.Title);
    }

    [Fact]
    public async Task GetAutocomplete_ReturnsOkWithResults_WhenRequestIsValid()
    {
        // Arrange
        var request = new NotesGetAutocompleteRequest
        {
            SearchTerm = "test",
            Section = "ValidSection"
        };

        mockSectionsService.Setup(s => s.IsSectionNameValid(request.Section, cancellationToken))
            .ReturnsAsync(true);

        var notes = new List<NoteEntity>
        {
            new() { Id = 1, Section = "ValidSection", Title = "Test Note 1" },
            new() { Id = 2, Section = "ValidSection", Title = "Test Note 2" }
        };

        DbContext.Notes.AddRange(notes);
        await DbContext.SaveChangesAsync(cancellationToken);

        mockBooksService.Setup(b => b.Autocomplete(request.SearchTerm, request.Section, cancellationToken))
            .ReturnsAsync([(1, "Test Book")]);

        mockTagsService.Setup(t => t.Autocomplete(request.SearchTerm, request.Section, cancellationToken))
            .ReturnsAsync([("1", "TestTag")]);

        // Act
        var result = await controller.GetAutocomplete(request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<NotesAutocompleteResponse>(okResult.Value);
        Assert.Equal(2, response.Notes.Count);
        Assert.Single(response.Books);
        Assert.Single(response.Tags);
    }

    #endregion

    #region Calendar Tests

    [Fact]
    public async Task GetCalendarDays_ReturnsValidationProblem_WhenSectionIsNotValid()
    {
        // Arrange
        var request = new NotesGetCalendarDaysRequest
        {
            Month = 1,
            Year = 2023,
            Section = "InvalidSection"
        };

        mockSectionsService.Setup(s => s.IsSectionNameValid(request.Section, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await controller.GetCalendarDays(request, cancellationToken);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var validationProblem = Assert.IsType<ValidationProblemDetails>(objectResult.Value);
        Assert.Equal($"Section '{request.Section}' is not valid!", validationProblem.Title);
    }

    [Fact]
    public async Task GetCalendarDays_ReturnsOkWithResults_WhenRequestIsValid()
    {
        // Arrange
        var year = 2023;
        var month = 1;
        var request = new NotesGetCalendarDaysRequest
        {
            Month = month,
            Year = year,
            Section = "ValidSection"
        };

        mockSectionsService.Setup(s => s.IsSectionNameValid(request.Section, cancellationToken))
            .ReturnsAsync(true);

        // Use direct SQL to insert notes with specific timestamps
        var date1 = new DateTime(year, month, 1, 12, 0, 0, DateTimeKind.Utc);
        var date2 = new DateTime(year, month, 1, 14, 0, 0, DateTimeKind.Utc);
        var date3 = new DateTime(year, month, 2, 9, 0, 0, DateTimeKind.Utc);

        await DbContext.Database.ExecuteSqlRawAsync(
            @"INSERT INTO Notes (
            Id, Section, Title, CreatedAt, UpdatedAt, 
            Tags, Sources, Content, TitleSearchIndex, ContentSearchIndex, Preview, Book
        ) VALUES (
            1, 'ValidSection', 'Note 1', {0}, {0}, 
            '', '[]', NULL, NULL, NULL, NULL, NULL
        )",
            date1);

        await DbContext.Database.ExecuteSqlRawAsync(
            @"INSERT INTO Notes (
            Id, Section, Title, CreatedAt, UpdatedAt, 
            Tags, Sources, Content, TitleSearchIndex, ContentSearchIndex, Preview, Book
        ) VALUES (
            2, 'ValidSection', 'Note 2', {0}, {0}, 
            '', '[]', NULL, NULL, NULL, NULL, NULL
        )",
            date2);

        await DbContext.Database.ExecuteSqlRawAsync(
            @"INSERT INTO Notes (
            Id, Section, Title, CreatedAt, UpdatedAt, 
            Tags, Sources, Content, TitleSearchIndex, ContentSearchIndex, Preview, Book
        ) VALUES (
            3, 'ValidSection', 'Note 3', {0}, {0}, 
            '', '[]', NULL, NULL, NULL, NULL, NULL
        )",
            date3);
        

        // Act
        var result = await controller.GetCalendarDays(request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<IEnumerable<NotesCalendarDaysResponseItem>>(okResult.Value);

        var responseList = response.ToList();
        Assert.Equal(2, responseList.Count); // Two days with notes

        var day1 = responseList.FirstOrDefault(d => d.Date == $"{year}-{month:D2}-01");
        Assert.NotNull(day1);
        Assert.Equal(2, day1.Count);

        var day2 = responseList.FirstOrDefault(d => d.Date == $"{year}-{month:D2}-02");
        Assert.NotNull(day2);
        Assert.Equal(1, day2.Count);
    }

    #endregion
}