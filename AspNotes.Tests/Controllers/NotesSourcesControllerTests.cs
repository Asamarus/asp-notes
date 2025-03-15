using AspNotes.Controllers;
using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AspNotes.Tests.Controllers;
public class NotesSourcesControllerTests : InMemoryDatabaseTestBase
{
    private readonly Mock<IUrlMetadataHelper> mockUrlMetadataHelper;
    private readonly NotesSourcesController controller;
    private readonly CancellationToken cancellationToken = CancellationToken.None;

    public NotesSourcesControllerTests()
    {
        mockUrlMetadataHelper = new Mock<IUrlMetadataHelper>();
        controller = new NotesSourcesController(DbContext, mockUrlMetadataHelper.Object);
    }

    [Fact]
    public async Task AddNoteSource_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        long noteId = 999;
        var request = new NotesSourceCreateRequest { Link = "https://example.com" };

        // Act
        var result = await controller.AddNoteSource(noteId, request, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddNoteSource_ReturnsCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection"
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var request = new NotesSourceCreateRequest { Link = "https://example.com" };

        var metadata = new HtmlDocumentMetadata
        {
            Title = "Test Title",
            Description = "Test Description",
            Image = "https://example.com/image.jpg"
        };

        mockUrlMetadataHelper.Setup(u => u.GetUrlMetadata(request.Link, cancellationToken))
            .ReturnsAsync(metadata);

        // Act
        var result = await controller.AddNoteSource(noteEntity.Id, request, cancellationToken);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var sources = Assert.IsAssignableFrom<List<NotesSource>>(createdAtActionResult.Value);
        Assert.Single(sources);
        Assert.Equal(request.Link, sources[0].Link);
        Assert.Equal(metadata.Title, sources[0].Title);
        Assert.Equal(metadata.Image, sources[0].Image);
        Assert.True(sources[0].ShowImage);

        // Verify the source was added to the database
        var updatedNote = await DbContext.Notes.FindAsync(noteEntity.Id);
        Assert.NotNull(updatedNote);
        Assert.Single(updatedNote.Sources);
    }

    [Fact]
    public async Task AddNoteSource_TruncatesLongDescription_WhenDescriptionIsLongerThan100Chars()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection"
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var request = new NotesSourceCreateRequest { Link = "https://example.com" };

        var longDescription = new string('a', 150);
        var metadata = new HtmlDocumentMetadata
        {
            Title = "Test Title",
            Description = longDescription,
            Image = "https://example.com/image.jpg"
        };

        mockUrlMetadataHelper.Setup(u => u.GetUrlMetadata(request.Link, cancellationToken))
            .ReturnsAsync(metadata);

        // Act
        var result = await controller.AddNoteSource(noteEntity.Id, request, cancellationToken);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var sources = Assert.IsAssignableFrom<List<NotesSource>>(createdAtActionResult.Value);
        Assert.Single(sources);
        Assert.NotNull(sources[0].Description);
        Assert.Equal(100 + 3, sources[0].Description?.Length); // 100 chars + "..."
        Assert.EndsWith("...", sources[0].Description);
    }

    [Fact]
    public async Task UpdateNoteSource_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        long noteId = 999;
        string sourceId = "source-id";
        var request = new NotesSourceUpdateRequest
        {
            Link = "https://example.com",
            Title = "Updated Title",
            Description = "Updated Description",
            Image = "https://example.com/updated.jpg",
            ShowImage = true
        };

        // Act
        var result = await controller.UpdateNoteSource(noteId, sourceId, request, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateNoteSource_ReturnsNotFound_WhenSourceDoesNotExist()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection",
            Sources =
            [
                new() { Id = "existing-id", Link = "https://old.com" }
            ]
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        string nonExistentSourceId = "nonexistent-source-id";
        var request = new NotesSourceUpdateRequest
        {
            Link = "https://example.com",
            Title = "Updated Title",
            Description = "Updated Description",
            Image = "https://example.com/updated.jpg",
            ShowImage = true
        };

        // Act
        var result = await controller.UpdateNoteSource(noteEntity.Id, nonExistentSourceId, request, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateNoteSource_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var sourceId = Guid.NewGuid().ToString();
        var noteEntity = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection",
            Sources =
            [
                new() {
                    Id = sourceId,
                    Link = "https://old.com",
                    Title = "Old Title",
                    Description = "Old Description",
                    ShowImage = false
                }
            ]
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var request = new NotesSourceUpdateRequest
        {
            Link = "https://example.com",
            Title = "Updated Title",
            Description = "Updated Description",
            ShowImage = true
        };

        // Act
        var result = await controller.UpdateNoteSource(noteEntity.Id, sourceId, request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var sources = Assert.IsAssignableFrom<List<NotesSource>>(okResult.Value);
        Assert.Single(sources);
        Assert.Equal(request.Link, sources[0].Link);
        Assert.Equal(request.Title, sources[0].Title);
        Assert.Equal(request.Description, sources[0].Description);
        Assert.Equal(request.ShowImage, sources[0].ShowImage);

        // Verify the source was updated in the database
        var updatedNote = await DbContext.Notes.FindAsync(noteEntity.Id);
        Assert.NotNull(updatedNote);
        Assert.Single(updatedNote.Sources);
        Assert.Equal(request.Link, updatedNote.Sources[0].Link);
        Assert.Equal(request.Title, updatedNote.Sources[0].Title);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        long noteId = 999;
        string sourceId = "source-id";

        // Act
        var result = await controller.Delete(noteId, sourceId, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenSourceDoesNotExist()
    {
        // Arrange
        var noteEntity = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection",
            Sources =
            [
                new() { Id = "existing-id", Link = "https://example.com" }
            ]
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        string nonExistentSourceId = "nonexistent-source-id";

        // Act
        var result = await controller.Delete(noteEntity.Id, nonExistentSourceId, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var sourceId = Guid.NewGuid().ToString();
        var noteEntity = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection",
            Sources =
            [
                new() { Id = sourceId, Link = "https://example.com" }
            ]
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        // Act
        var result = await controller.Delete(noteEntity.Id, sourceId, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var sources = Assert.IsAssignableFrom<List<NotesSource>>(okResult.Value);
        Assert.Empty(sources);

        // Verify the source was removed from the database
        var updatedNote = await DbContext.Notes.FindAsync(noteEntity.Id);
        Assert.NotNull(updatedNote);
        Assert.Empty(updatedNote.Sources);
    }

    [Fact]
    public async Task Reorder_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        // Arrange
        long noteId = 999;
        var request = new NotesSourcesReorderRequest
        {
            SourceIds = ["source-id-1", "source-id-2"]
        };

        // Act
        var result = await controller.Reorder(noteId, request, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Reorder_ReturnsValidationProblem_WhenSourceDoesNotExist()
    {
        // Arrange
        var sourceId1 = Guid.NewGuid().ToString();
        var sourceId2 = Guid.NewGuid().ToString();
        var noteEntity = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection",
            Sources =
            [
                new() { Id = sourceId1, Link = "https://example1.com" },
                new() { Id = sourceId2, Link = "https://example2.com" }
            ]
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var request = new NotesSourcesReorderRequest
        {
            SourceIds = [sourceId1, "nonexistent-source-id"]
        };

        // Act
        var result = await controller.Reorder(noteEntity.Id, request, cancellationToken);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var validationProblem = Assert.IsType<ValidationProblemDetails>(objectResult.Value);
        Assert.Equal($"Source with id 'nonexistent-source-id' is not found!", validationProblem.Title);
    }

    [Fact]
    public async Task Reorder_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var sourceId1 = Guid.NewGuid().ToString();
        var sourceId2 = Guid.NewGuid().ToString();
        var noteEntity = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "TestSection",
            Sources =
            [
                new() { Id = sourceId1, Link = "https://example1.com" },
                new() { Id = sourceId2, Link = "https://example2.com" }
            ]
        };

        DbContext.Notes.Add(noteEntity);
        await DbContext.SaveChangesAsync(cancellationToken);

        var request = new NotesSourcesReorderRequest
        {
            SourceIds = [sourceId2, sourceId1] // Reversed order
        };

        // Act
        var result = await controller.Reorder(noteEntity.Id, request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var sources = Assert.IsAssignableFrom<List<NotesSource>>(okResult.Value);
        Assert.Equal(2, sources.Count);
        Assert.Equal(sourceId2, sources[0].Id); // Now first
        Assert.Equal(sourceId1, sources[1].Id); // Now second

        // Verify the sources were reordered in the database
        var updatedNote = await DbContext.Notes.FindAsync(noteEntity.Id);
        Assert.NotNull(updatedNote);
        Assert.Equal(2, updatedNote.Sources.Count);
        Assert.Equal(sourceId2, updatedNote.Sources[0].Id);
        Assert.Equal(sourceId1, updatedNote.Sources[1].Id);
    }
}
