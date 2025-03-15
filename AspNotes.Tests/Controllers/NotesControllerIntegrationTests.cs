using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AspNotes.Tests.Controllers;

public class NotesControllerIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    private readonly ApiFactory factory;

    public NotesControllerIntegrationTests(ApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());
    }

    #region Get Tests

    [Fact]
    public async Task Get_ReturnsOkResult_WithPaginatedNotes()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await client.GetAsync("/api/notes");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<NotesItemResponse>>();

        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result.LastPage);
        Assert.False(result.CanLoadMore);
        Assert.Equal(1, result.Page);
        Assert.NotEmpty(result.Data);
    }

    [Fact]
    public async Task Get_FiltersBySection_WhenSectionIsProvided()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await client.GetAsync("/api/notes?section=TestSection1");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<NotesItemResponse>>();

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Data);
        Assert.Equal("TestSection1", result.Data.First().Section);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenInvalidSectionIsProvided()
    {
        // Arrange
        var invalidSection = "NonExistentSection";

        // Act
        var response = await client.GetAsync($"/api/notes?section={invalidSection}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal($"Section '{invalidSection}' is not valid!", problemDetails.Title);
    }

    [Fact]
    public async Task Get_FiltersByBook_WhenBookIsProvided()
    {
        // Arrange
        await SeedTestDataWithBooksAsync();

        // Act
        var response = await client.GetAsync("/api/notes?book=TestBook");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<NotesItemResponse>>();

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Data);
        Assert.Equal("TestBook", result.Data.First().Book);
    }

    [Fact]
    public async Task Get_FiltersByTags_WhenTagsAreProvided()
    {
        // Arrange
        await SeedTestDataWithBooksAndTagsAsync();

        // Act
        var response = await client.GetAsync("/api/notes?tags=TestTag1");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<NotesItemResponse>>();

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Data);
        Assert.Contains("TestTag1", result.Data.First().Tags);
    }

    [Fact]
    public async Task Get_FiltersWithoutBooks_WhenWithoutBookIsTrue()
    {
        // Arrange
        await SeedTestDataWithBooksAsync();

        // Act
        var response = await client.GetAsync("/api/notes?withoutBook=true");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<NotesItemResponse>>();

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Data);
        Assert.Null(result.Data.First().Book);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenNoteExists()
    {
        // Arrange
        var noteId = await SeedTestNoteAsync("TestNote", "TestSection1");

        // Act
        var response = await client.GetAsync($"/api/notes/{noteId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<NotesItemResponse>();

        Assert.NotNull(result);
        Assert.Equal(noteId, result.Id);
        Assert.Equal("TestNote", result.Title);
        Assert.Equal("TestSection1", result.Section);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Act
        var response = await client.GetAsync("/api/notes/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Post Tests

    [Fact]
    public async Task Post_CreatesNote_WhenRequestIsValid()
    {
        // Arrange
        await SeedTestSectionsAsync();
        var request = new NotesCreateRequest
        {
            Section = "TestSection1"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/notes", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<NotesItemResponse>();

        Assert.NotNull(result);
        Assert.Equal("TestSection1", result.Section);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenSectionIsInvalid()
    {
        // Arrange
        var request = new NotesCreateRequest
        {
            Section = "NonExistentSection"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/notes", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal($"Section '{request.Section}' is not valid!", problemDetails.Title);
    }

    [Fact]
    public async Task Post_CreatesNoteWithBook_WhenBookIsProvided()
    {
        // Arrange
        await SeedTestSectionsAsync();
        var request = new NotesCreateRequest
        {
            Section = "TestSection1",
            Book = "NewTestBook"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/notes", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<NotesItemResponse>();

        Assert.NotNull(result);
        Assert.Equal("TestSection1", result.Section);
        Assert.Equal("NewTestBook", result.Book);
    }

    #endregion

    #region Patch Tests

    [Fact]
    public async Task Patch_UpdatesNote_WhenRequestIsValid()
    {
        // Arrange
        var noteId = await SeedTestNoteAsync("Original Title", "TestSection1");
        var request = new NotesPatchRequest
        {
            Title = "Updated Title",
            Content = "Updated Content"
        };

        // Act
        var response = await client.PatchAsJsonAsync($"/api/notes/{noteId}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<NotesItemResponse>();

        Assert.NotNull(result);
        Assert.Equal(noteId, result.Id);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated Content", result.Content);
    }

    [Fact]
    public async Task Patch_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        var request = new NotesPatchRequest
        {
            Title = "Updated Title"
        };

        // Act
        var response = await client.PatchAsJsonAsync("/api/notes/999999", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Patch_ReturnsBadRequest_WhenSectionIsInvalid()
    {
        // Arrange
        var noteId = await SeedTestNoteAsync("Test Note", "TestSection1");
        var request = new NotesPatchRequest
        {
            Section = "NonExistentSection"
        };

        // Act
        var response = await client.PatchAsJsonAsync($"/api/notes/{noteId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal($"Section '{request.Section}' is not valid!", problemDetails.Title);
    }

    [Fact]
    public async Task Patch_UpdatesTags_WhenTagsAreProvided()
    {
        // Arrange
        var noteId = await SeedTestNoteAsync("Note with Tags", "TestSection1");
        var request = new NotesPatchRequest
        {
            Tags = ["NewTag1", "NewTag2"]
        };

        // Act
        var response = await client.PatchAsJsonAsync($"/api/notes/{noteId}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<NotesItemResponse>();

        Assert.NotNull(result);
        Assert.Equal(2, result.Tags.Count);
        Assert.Contains("NewTag1", result.Tags);
        Assert.Contains("NewTag2", result.Tags);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_RemovesNote_WhenNoteExists()
    {
        // Arrange
        var noteId = await SeedTestNoteAsync("Note to Delete", "TestSection1");

        // Act
        var response = await client.DeleteAsync($"/api/notes/{noteId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify note was deleted
        var getResponse = await client.GetAsync($"/api/notes/{noteId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Act
        var response = await client.DeleteAsync("/api/notes/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Autocomplete Tests

    [Fact]
    public async Task GetAutocomplete_ReturnsOkResult_WithSuggestions()
    {
        // Arrange
        await SeedTestDataWithBooksAndTagsAsync();

        // Act
        var response = await client.GetAsync("/api/notes/autocomplete?searchTerm=Test");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<NotesAutocompleteResponse>();

        Assert.NotNull(result);
        Assert.NotEmpty(result.Notes);
        Assert.NotEmpty(result.Books);
        Assert.NotEmpty(result.Tags);
    }

    [Fact]
    public async Task GetAutocomplete_FiltersBySection_WhenSectionIsProvided()
    {
        // Arrange
        await SeedTestDataWithBooksAndTagsAsync();

        // Act
        var response = await client.GetAsync("/api/notes/autocomplete?searchTerm=Test&section=TestSection1");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<NotesAutocompleteResponse>();

        Assert.NotNull(result);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var noteIds = result.Notes.Select(note => note.Id).ToList();
        var noteEntities = dbContext.Notes.Where(note => noteIds.Contains(note.Id)).ToList();

        Assert.All(result.Notes, note =>
        {
            var noteEntity = noteEntities.FirstOrDefault(ne => ne.Id == note.Id);
            Assert.NotNull(noteEntity);
            Assert.Equal("TestSection1", noteEntity.Section);
        });
    }

    [Fact]
    public async Task GetAutocomplete_ReturnsBadRequest_WhenSearchTermIsTooShort()
    {
        // Act
        var response = await client.GetAsync("/api/notes/autocomplete?searchTerm=A");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Contains("SearchTerm", problemDetails.Errors.Keys);
    }

    #endregion

    #region Calendar Tests

    [Fact]
    public async Task GetCalendarDays_ReturnsOkResult_WithDayCounts()
    {
        // Arrange
        await SeedTestDataAsync();
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        // Act
        var response = await client.GetAsync($"/api/notes/calendar?month={currentMonth}&year={currentYear}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<NotesCalendarDaysResponseItem>>();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, item =>
        {
            Assert.True(item.Count > 0);
            Assert.NotNull(item.Date);
            var date = DateTime.ParseExact(item.Date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal(currentMonth, date.Month);
            Assert.Equal(currentYear, date.Year);
        });
    }

    [Fact]
    public async Task GetCalendarDays_FiltersBySection_WhenSectionIsProvided()
    {
        // Arrange
        await SeedTestDataAsync();
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        // Act
        var response = await client.GetAsync($"/api/notes/calendar?month={currentMonth}&year={currentYear}&section=TestSection1");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<NotesCalendarDaysResponseItem>>();

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCalendarDays_ReturnsBadRequest_WhenInvalidYearIsProvided()
    {
        // Act
        var response = await client.GetAsync("/api/notes/calendar?month=1&year=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Contains("Year", problemDetails.Errors.Keys);
    }

    #endregion

    #region Helper Methods

    private async Task SeedTestSectionsAsync()
    {
        using var scope = factory.Services.CreateScope();
        var sectionsService = scope.ServiceProvider.GetRequiredService<ISectionsService>();

        // Clear any existing sections first
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        dbContext.Sections.RemoveRange(dbContext.Sections.ToList());
        await dbContext.SaveChangesAsync(CancellationToken.None);

        await sectionsService.CreateSection("TestSection1", "Test Section 1", "#111111", CancellationToken.None);
        await sectionsService.CreateSection("TestSection2", "Test Section 2", "#222222", CancellationToken.None);
    }

    private async Task<long> SeedTestNoteAsync(string title, string section)
    {
        await SeedTestSectionsAsync();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var note = new NoteEntity
        {
            Title = title,
            Content = "Test content",
            Section = section,
            TitleSearchIndex = title,
            ContentSearchIndex = "Test content"
        };

        await dbContext.Notes.AddAsync(note);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        return note.Id;
    }

    private async Task SeedTestDataAsync()
    {
        await SeedTestSectionsAsync();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        // Clear any existing notes first
        dbContext.Notes.RemoveRange(dbContext.Notes.ToList());
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var note1 = new NoteEntity
        {
            Title = "Test Note 1",
            Content = "Test content 1",
            Section = "TestSection1",
            TitleSearchIndex = "Test Note 1",
            ContentSearchIndex = "Test content 1"
        };

        var note2 = new NoteEntity
        {
            Title = "Test Note 2",
            Content = "Test content 2",
            Section = "TestSection2",
            TitleSearchIndex = "Test Note 2",
            ContentSearchIndex = "Test content 2"
        };

        await dbContext.Notes.AddRangeAsync(note1, note2);
        await dbContext.SaveChangesAsync(CancellationToken.None);
    }

    private async Task SeedTestDataWithBooksAsync()
    {
        await SeedTestDataAsync();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var booksService = scope.ServiceProvider.GetRequiredService<IBooksService>();

        var notes = dbContext.Notes.ToList();
        await booksService.UpdateNoteBook(notes[0].Id, "TestBook", CancellationToken.None);
    }

    private async Task SeedTestDataWithBooksAndTagsAsync()
    {
        await SeedTestDataWithBooksAsync();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var tagsService = scope.ServiceProvider.GetRequiredService<ITagsService>();

        var notes = dbContext.Notes.ToList();
        await tagsService.UpdateNoteTags(notes[0].Id, ["TestTag1", "TestTag2"], CancellationToken.None);
        await tagsService.UpdateNoteTags(notes[1].Id, ["TestTag3"], CancellationToken.None);
    }

    #endregion
}