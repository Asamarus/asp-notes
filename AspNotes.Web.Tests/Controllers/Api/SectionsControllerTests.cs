using AspNotes.Core.Section;
using AspNotes.Core.Section.Models;
using AspNotes.Web.Controllers.Api;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Sections;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AspNotes.Web.Tests.Controllers.Api;

public class SectionsControllerTests
{
    [Fact]
    public async Task GetSectionsList_ReturnsAllSections()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        var expectedSections = new List<SectionDto>
        {
            new() { Id = 1, Name = "Section1", DisplayName = "Display Name 1", Color = "Red", Position = 1 },
            new() { Id = 2, Name = "Section2", DisplayName = "Display Name 2", Color = "Blue", Position = 2 }
        };
        mockService.Setup(service => service.GetSections()).ReturnsAsync(expectedSections);

        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.GetSectionsList();
        var okResult = result as OkObjectResult;
        var sectionsResponse = okResult?.Value as SectionsResponse;

        // Assert
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        Assert.NotNull(sectionsResponse);
        Assert.Equal(expectedSections.Count, sectionsResponse.Sections.Count);
        Assert.True(expectedSections.All(expectedSection => sectionsResponse.Sections.Any(sr => sr.Id == expectedSection.Id && sr.Name == expectedSection.Name)));
    }

    [Fact]
    public async Task CreateSection_NameNotUnique_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        mockService.Setup(s => s.IsSectionNameUnique(It.IsAny<string>())).ReturnsAsync(false);
        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.CreateSection(new CreateSectionRequest { Name = "test", DisplayName = "Test Section", Color = "#FFFFFF" });

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Section name is not unique!", errorResponse.Message);
    }

    [Fact]
    public async Task CreateSection_SuccessfulCreation_ReturnsOk()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        mockService.Setup(s => s.IsSectionNameUnique(It.IsAny<string>())).ReturnsAsync(true);
        mockService.Setup(s => s.CreateSection(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(1L);
        mockService.Setup(s => s.GetSections()).ReturnsAsync(
        [
            new SectionDto { Id = 1, Name = "test", DisplayName = "Test Section", Color = "#FFFFFF" }
        ]);
        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.CreateSection(new CreateSectionRequest { Name = "test", DisplayName = "Test Section", Color = "#FFFFFF" });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var sectionsResponse = Assert.IsType<SectionsResponse>(okResult.Value);
        Assert.Single(sectionsResponse.Sections);
        Assert.Equal("Section is created successfully!", sectionsResponse.Message);
    }

    [Fact]
    public async Task UpdateSection_SectionIdNotPresent_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        mockService.Setup(service => service.IsSectionIdPresent(It.IsAny<long>())).ReturnsAsync(false);
        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.UpdateSection(new UpdateSectionRequest { Id = 1 });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateSection_UpdateFails_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        mockService.Setup(service => service.IsSectionIdPresent(It.IsAny<long>())).ReturnsAsync(true);
        mockService.Setup(service => service.UpdateSection(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.UpdateSection(new UpdateSectionRequest { Id = 1 });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateSection_Success_ReturnsOkWithSections()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        mockService.Setup(service => service.IsSectionIdPresent(It.IsAny<long>())).ReturnsAsync(true);
        mockService.Setup(service => service.UpdateSection(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        mockService.Setup(service => service.GetSections()).ReturnsAsync(
        [
            new SectionDto { Id = 1, Name = "Test", DisplayName = "Test Display", Color = "#000000" }
        ]);
        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.UpdateSection(new UpdateSectionRequest { Id = 1, DisplayName = "Test Display", Color = "#000000" });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SectionsResponse>(okResult.Value);
        Assert.Single(response.Sections);
        Assert.Equal("Test Display", response.Sections.First().DisplayName);
        Assert.Equal("#000000", response.Sections.First().Color);
    }

    [Fact]
    public async Task DeleteSection_SectionNotFound_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        mockService.Setup(s => s.IsSectionIdPresent(It.IsAny<long>())).ReturnsAsync(false);
        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.DeleteSection(new DeleteSectionRequest { Id = 999 });

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<ErrorResponse>(badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteSection_DeletionFails_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        mockService.Setup(s => s.IsSectionIdPresent(It.IsAny<long>())).ReturnsAsync(true);
        mockService.Setup(s => s.DeleteSection(It.IsAny<long>())).ReturnsAsync(false);
        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.DeleteSection(new DeleteSectionRequest { Id = 1 });

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<ErrorResponse>(badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteSection_SuccessfulDeletion_ReturnsOk()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        mockService.Setup(s => s.IsSectionIdPresent(It.IsAny<long>())).ReturnsAsync(true);
        mockService.Setup(s => s.DeleteSection(It.IsAny<long>())).ReturnsAsync(true);
        mockService.Setup(s => s.GetSections()).ReturnsAsync([]);
        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.DeleteSection(new DeleteSectionRequest { Id = 1 });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<SectionsResponse>(okResult.Value);
        var response = Assert.IsType<SectionsResponse>(okResult.Value);
        Assert.Empty(response.Sections);
    }

    [Fact]
    public async Task ReorderSections_ReorderFails_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        mockService.Setup(s => s.ReorderSections(It.IsAny<List<long>>())).ReturnsAsync(false);
        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.ReorderSections(new ReorderSectionsRequest { Ids = [1, 2, 3] });

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<ErrorResponse>(badRequestResult.Value);
    }

    [Fact]
    public async Task ReorderSections_SuccessfulReorder_ReturnsOk()
    {
        // Arrange
        var mockService = new Mock<ISectionsService>();
        mockService.Setup(s => s.ReorderSections(It.IsAny<List<long>>())).ReturnsAsync(true);
        mockService.Setup(s => s.GetSections()).ReturnsAsync(
        [
            new SectionDto { Id = 1, Name = "Section 1", DisplayName = "Section One", Color = "#000" },
            new SectionDto { Id = 2, Name = "Section 2", DisplayName = "Section Two", Color = "#ccc" }
        ]);
        var controller = new SectionsController(mockService.Object);

        // Act
        var result = await controller.ReorderSections(new ReorderSectionsRequest { Ids = [1, 2] });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SectionsResponse>(okResult.Value);
        Assert.Equal(2, response.Sections.Count);
        Assert.Equal("Sections are reordered successfully!", response.Message);
    }
}