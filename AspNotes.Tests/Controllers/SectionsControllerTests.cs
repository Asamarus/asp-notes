using AspNotes.Controllers;
using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AspNotes.Tests.Controllers;

public class SectionsControllerTests
{
    private readonly Mock<ISectionsService> mockSectionsService;
    private readonly SectionsController controller;
    private readonly CancellationToken cancellationToken = CancellationToken.None;

    public SectionsControllerTests()
    {
        mockSectionsService = new Mock<ISectionsService>();
        controller = new SectionsController(mockSectionsService.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResultWithSections_WhenCalled()
    {
        // Arrange
        var sections = new List<SectionEntity>
        {
            new() { Id = 1, Name = "test", DisplayName = "Test", Color = "#000000", Position = 0 },
            new() { Id = 2, Name = "test2", DisplayName = "Test 2", Color = "#FFFFFF", Position = 1 }
        };

        mockSectionsService.Setup(s => s.GetSections(cancellationToken))
            .ReturnsAsync(sections);

        // Act
        var result = await controller.Get(cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<SectionsItemResponse>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtActionResult_WhenNameIsUnique()
    {
        // Arrange
        var request = new SectionsCreateRequest
        {
            Name = "test",
            DisplayName = "Test",
            Color = "#000000"
        };

        var sections = new List<SectionEntity>
        {
            new() { Id = 1, Name = "test", DisplayName = "Test", Color = "#000000", Position = 0 }
        };

        mockSectionsService.Setup(s => s.IsSectionNameUnique(request.Name, cancellationToken))
            .ReturnsAsync(true);
        mockSectionsService.Setup(s => s.GetSections(cancellationToken))
            .ReturnsAsync(sections);

        // Act
        var result = await controller.Post(request, cancellationToken);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(SectionsController.Get), createdAtActionResult.ActionName);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<SectionsItemResponse>>(createdAtActionResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task Post_ReturnsValidationProblem_WhenNameIsDuplicate()
    {
        // Arrange
        var request = new SectionsCreateRequest
        {
            Name = "duplicate",
            DisplayName = "Duplicate",
            Color = "#000000"
        };

        mockSectionsService.Setup(s => s.IsSectionNameUnique(request.Name, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await controller.Post(request, cancellationToken);

        // Assert
        var validationResult = Assert.IsType<ObjectResult>(result);
        var validationProblemDetails = Assert.IsType<ValidationProblemDetails>(validationResult.Value);
        Assert.Equal("Section name is not unique!", validationProblemDetails.Title);
    }

    [Fact]
    public async Task Put_ReturnsOkResult_WhenIdExists()
    {
        // Arrange
        long id = 1;
        var request = new SectionsUpdateRequest
        {
            DisplayName = "Updated Test",
            Color = "#FFFFFF"
        };

        var sections = new List<SectionEntity>
        {
            new() { Id = 1, Name = "test", DisplayName = "Updated Test", Color = "#FFFFFF", Position = 0 }
        };

        mockSectionsService.Setup(s => s.IsSectionIdPresent(id, cancellationToken))
            .ReturnsAsync(true);
        mockSectionsService.Setup(s => s.GetSections(cancellationToken))
            .ReturnsAsync(sections);

        // Act
        var result = await controller.Put(id, request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<SectionsItemResponse>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenIdDoesNotExist()
    {
        // Arrange
        long id = 999;
        var request = new SectionsUpdateRequest
        {
            DisplayName = "Updated Test",
            Color = "#FFFFFF"
        };

        mockSectionsService.Setup(s => s.IsSectionIdPresent(id, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await controller.Put(id, request, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOkResult_WhenIdExistsAndNoNotes()
    {
        // Arrange
        long id = 1;
        var sections = new List<SectionEntity>
        {
            new() { Id = 2, Name = "test2", DisplayName = "Test 2", Color = "#FFFFFF", Position = 0 }
        };

        mockSectionsService.Setup(s => s.IsSectionIdPresent(id, cancellationToken))
            .ReturnsAsync(true);
        mockSectionsService.Setup(s => s.IsSectionHavingNotes(id, cancellationToken))
            .ReturnsAsync(false);
        mockSectionsService.Setup(s => s.GetSections(cancellationToken))
            .ReturnsAsync(sections);

        // Act
        var result = await controller.Delete(id, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<SectionsItemResponse>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenIdDoesNotExist()
    {
        // Arrange
        long id = 999;

        mockSectionsService.Setup(s => s.IsSectionIdPresent(id, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await controller.Delete(id, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsValidationProblem_WhenSectionHasNotes()
    {
        // Arrange
        long id = 1;

        mockSectionsService.Setup(s => s.IsSectionIdPresent(id, cancellationToken))
            .ReturnsAsync(true);
        mockSectionsService.Setup(s => s.IsSectionHavingNotes(id, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await controller.Delete(id, cancellationToken);

        // Assert
        var validationResult = Assert.IsType<ObjectResult>(result);
        var validationProblemDetails = Assert.IsType<ValidationProblemDetails>(validationResult.Value);
        Assert.Equal("Section has notes!", validationProblemDetails.Title);
    }

    [Fact]
    public async Task Reorder_ReturnsOkResult_WhenIdsAreValid()
    {
        // Arrange
        var request = new SectionsReorderRequest
        {
            Ids = [2, 1]
        };

        var sections = new List<SectionEntity>
        {
            new() { Id = 2, Name = "test2", DisplayName = "Test 2", Color = "#FFFFFF", Position = 0 },
            new() { Id = 1, Name = "test", DisplayName = "Test", Color = "#000000", Position = 1 }
        };

        mockSectionsService.Setup(s => s.ReorderSections(request.Ids, cancellationToken))
            .ReturnsAsync(true);
        mockSectionsService.Setup(s => s.GetSections(cancellationToken))
            .ReturnsAsync(sections);

        // Act
        var result = await controller.Reorder(request, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<SectionsItemResponse>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task Reorder_ReturnsValidationProblem_WhenIdsAreInvalid()
    {
        // Arrange
        var request = new SectionsReorderRequest
        {
            Ids = [999, 1]
        };

        mockSectionsService.Setup(s => s.ReorderSections(request.Ids, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await controller.Reorder(request, cancellationToken);

        // Assert
        var validationResult = Assert.IsType<ObjectResult>(result);
        var validationProblemDetails = Assert.IsType<ValidationProblemDetails>(validationResult.Value);
        Assert.Equal("Sections are not reordered!", validationProblemDetails.Title);
    }
}
