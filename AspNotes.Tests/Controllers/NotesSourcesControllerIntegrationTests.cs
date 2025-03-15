using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AspNotes.Tests.Controllers;

public class NotesSourcesControllerIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    private readonly ApiFactory factory;

    public NotesSourcesControllerIntegrationTests(ApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestHelper.GetTestUserToken());
    }

    [Fact]
    public async Task AddNoteSource_ShouldReturnCreated_WhenNoteExistsAndUrlIsValid()
    {
        // Arrange
        var noteId = await SeedTestNoteAsync();
        var request = new NotesSourceCreateRequest { Link = "https://example.com" };

        // Act
        var response = await client.PostAsJsonAsync($"/api/notes/{noteId}/sources", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var sources = await response.Content.ReadFromJsonAsync<List<NotesSource>>();

        Assert.NotNull(sources);
        Assert.Single(sources);
        Assert.Equal("https://example.com", sources[0].Link);
        Assert.NotNull(sources[0].Id);
    }

    [Fact]
    public async Task AddNoteSource_ShouldReturnNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        var nonExistingNoteId = 999999;
        var request = new NotesSourceCreateRequest { Link = "https://example.com" };

        // Act
        var response = await client.PostAsJsonAsync($"/api/notes/{nonExistingNoteId}/sources", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddNoteSource_ShouldReturnBadRequest_WhenUrlIsNotValid()
    {
        // Arrange
        var noteId = await SeedTestNoteAsync();
        var request = new NotesSourceCreateRequest { Link = "not-a-valid-url" };

        // Act
        var response = await client.PostAsJsonAsync($"/api/notes/{noteId}/sources", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Contains("Link", problemDetails.Errors.Keys);
    }

    [Fact]
    public async Task UpdateNoteSource_ShouldReturnOk_WhenSourceExists()
    {
        // Arrange
        var (noteId, sourceId) = await SeedTestNoteWithSourceAsync();
        var request = new NotesSourceUpdateRequest
        {
            Link = "https://updated-example.com",
            Title = "Updated Title",
            Description = "Updated Description",
            Image = "https://updated-example.com/image.jpg",
            ShowImage = true
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/notes/{noteId}/sources/{sourceId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sources = await response.Content.ReadFromJsonAsync<List<NotesSource>>();

        Assert.NotNull(sources);
        Assert.Single(sources);
        var updatedSource = sources[0];
        Assert.Equal(sourceId, updatedSource.Id);
        Assert.Equal(request.Link, updatedSource.Link);
        Assert.Equal(request.Title, updatedSource.Title);
        Assert.Equal(request.Description, updatedSource.Description);
        Assert.Equal(request.ShowImage, updatedSource.ShowImage);
    }

    [Fact]
    public async Task UpdateNoteSource_ShouldReturnNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        var nonExistingNoteId = 999999;
        var sourceId = "some-source-id";
        var request = new NotesSourceUpdateRequest
        {
            Link = "https://updated-example.com",
            Title = "Updated Title",
            Description = "Updated Description",
            Image = "https://updated-example.com/image.jpg",
            ShowImage = true
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/notes/{nonExistingNoteId}/sources/{sourceId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateNoteSource_ShouldReturnNotFound_WhenSourceDoesNotExist()
    {
        // Arrange
        var noteId = await SeedTestNoteAsync();
        var nonExistingSourceId = "non-existing-source-id";
        var request = new NotesSourceUpdateRequest
        {
            Link = "https://updated-example.com",
            Title = "Updated Title",
            Description = "Updated Description",
            Image = "https://updated-example.com/image.jpg",
            ShowImage = true
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/notes/{noteId}/sources/{nonExistingSourceId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenSourceExists()
    {
        // Arrange
        var (noteId, sourceId) = await SeedTestNoteWithSourceAsync();

        // Act
        var response = await client.DeleteAsync($"/api/notes/{noteId}/sources/{sourceId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sources = await response.Content.ReadFromJsonAsync<List<NotesSource>>();

        Assert.NotNull(sources);
        Assert.Empty(sources);

        // Verify in database
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var note = await dbContext.Notes.FindAsync(noteId);
        Assert.NotNull(note);
        Assert.Empty(note.Sources);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        var nonExistingNoteId = 999999;
        var sourceId = "some-source-id";

        // Act
        var response = await client.DeleteAsync($"/api/notes/{nonExistingNoteId}/sources/{sourceId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenSourceDoesNotExist()
    {
        // Arrange
        var noteId = await SeedTestNoteAsync();
        var nonExistingSourceId = "non-existing-source-id";

        // Act
        var response = await client.DeleteAsync($"/api/notes/{noteId}/sources/{nonExistingSourceId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Reorder_ShouldReturnOk_WhenSourcesExist()
    {
        // Arrange
        var (noteId, sourceIds) = await SeedTestNoteWithMultipleSourcesAsync();

        // Reverse the order
        sourceIds.Reverse();

        var request = new NotesSourcesReorderRequest
        {
            SourceIds = sourceIds
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/notes/{noteId}/sources/reorder", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sources = await response.Content.ReadFromJsonAsync<List<NotesSource>>();

        Assert.NotNull(sources);
        Assert.Equal(sourceIds.Count, sources.Count);

        // Verify order matches requested order
        for (int i = 0; i < sourceIds.Count; i++)
        {
            Assert.Equal(sourceIds[i], sources[i].Id);
        }
    }

    [Fact]
    public async Task Reorder_ShouldReturnNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        var nonExistingNoteId = 999999;
        var request = new NotesSourcesReorderRequest
        {
            SourceIds = ["source-id-1", "source-id-2"]
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/notes/{nonExistingNoteId}/sources/reorder", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Reorder_ShouldReturnBadRequest_WhenSourceDoesNotExist()
    {
        // Arrange
        var (noteId, sourceIds) = await SeedTestNoteWithMultipleSourcesAsync();

        // Add a non-existent source ID
        sourceIds.Add("non-existent-source-id");

        var request = new NotesSourcesReorderRequest
        {
            SourceIds = sourceIds
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/notes/{noteId}/sources/reorder", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Source with id 'non-existent-source-id' is not found!", problemDetails.Title);
    }

    private async Task<long> SeedTestNoteAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        dbContext.Sections.RemoveRange(dbContext.Sections.ToList());
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var sectionsService = scope.ServiceProvider.GetRequiredService<ISectionsService>();

        // Create a test section if needed
        await sectionsService.CreateSection("TestSection", "Test Section", "#000000", CancellationToken.None);

        // Create a test note
        var note = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection"
        };

        await dbContext.Notes.AddAsync(note);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        return note.Id;
    }

    private async Task<(long NoteId, string SourceId)> SeedTestNoteWithSourceAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        dbContext.Sections.RemoveRange(dbContext.Sections.ToList());
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var sectionsService = scope.ServiceProvider.GetRequiredService<ISectionsService>();

        // Create a test section if needed
        await sectionsService.CreateSection("TestSection", "Test Section", "#000000", CancellationToken.None);

        // Create a source
        var sourceId = Guid.NewGuid().ToString();
        var source = new NotesSource
        {
            Id = sourceId,
            Link = "https://example.com",
            Title = "Example Title",
            Description = "Example Description",
            ShowImage = false
        };

        // Create a test note with the source
        var note = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection",
            Sources = [source]
        };

        await dbContext.Notes.AddAsync(note);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        return (note.Id, sourceId);
    }

    private async Task<(long NoteId, List<string> SourceIds)> SeedTestNoteWithMultipleSourcesAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        dbContext.Sections.RemoveRange(dbContext.Sections.ToList());
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var sectionsService = scope.ServiceProvider.GetRequiredService<ISectionsService>();

        await sectionsService.CreateSection("TestSection", "Test Section", "#000000", CancellationToken.None);

        var sourceId1 = Guid.NewGuid().ToString();
        var sourceId2 = Guid.NewGuid().ToString();
        var sourceId3 = Guid.NewGuid().ToString();

        var sources = new List<NotesSource>
        {
            new() {
                Id = sourceId1,
                Link = "https://example1.com",
                Title = "Example Title 1",
                ShowImage = false
            },
            new() {
                Id = sourceId2,
                Link = "https://example2.com",
                Title = "Example Title 2",
                ShowImage = false
            },
            new() {
                Id = sourceId3,
                Link = "https://example3.com",
                Title = "Example Title 3",
                ShowImage = false
            }
        };

        var note = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection",
            Sources = sources
        };

        await dbContext.Notes.AddAsync(note);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        return (note.Id, new List<string> { sourceId1, sourceId2, sourceId3 });
    }
}
