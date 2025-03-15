using AspNotes.Controllers;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AspNotes.Tests.Controllers;

public class TagsControllerTests
{
    private readonly Mock<ITagsService> tagsServiceMock;
    private readonly Mock<ISectionsService> sectionsServiceMock;
    private readonly TagsController controller;

    public TagsControllerTests()
    {
        tagsServiceMock = new Mock<ITagsService>();
        sectionsServiceMock = new Mock<ISectionsService>();
        controller = new TagsController(tagsServiceMock.Object, sectionsServiceMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnOkResult_WhenSectionIsValid()
    {
        // Arrange
        var section = "validSection";
        var tags = new List<(long Id, string Name, int Count)>
        {
            (1, "Tag1", 2),
            (2, "Tag2", 3)
        };
        sectionsServiceMock.Setup(s => s.IsSectionNameValid(section, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        tagsServiceMock.Setup(t => t.GetTags(section, It.IsAny<CancellationToken>())).ReturnsAsync(tags);

        // Act
        var result = await controller.Get(section, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<ItemNameCountResponse>>(okResult.Value);
        Assert.Equal(2, response.Count);
        Assert.Equal("Tag1", response[0].Name);
        Assert.Equal(2, response[0].Count);
        Assert.Equal("Tag2", response[1].Name);
        Assert.Equal(3, response[1].Count);
    }

    [Fact]
    public async Task Get_ShouldReturnValidationProblem_WhenSectionIsInvalid()
    {
        // Arrange
        var section = "invalidSection";
        sectionsServiceMock.Setup(s => s.IsSectionNameValid(section, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await controller.Get(section, CancellationToken.None);

        // Assert
        var validationResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(validationResult.Value);
        Assert.Equal($"Section '{section}' is not valid!", problemDetails.Title);
    }

    [Fact]
    public async Task Get_ShouldReturnOkResult_WhenSectionIsNull()
    {
        // Arrange
        string? section = null;
        var tags = new List<(long Id, string Name, int Count)>
        {
            (1, "Tag1", 2),
            (2, "Tag2", 3)
        };
        tagsServiceMock.Setup(t => t.GetTags(section, It.IsAny<CancellationToken>())).ReturnsAsync(tags);

        // Act
        var result = await controller.Get(section, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<ItemNameCountResponse>>(okResult.Value);
        Assert.Equal(2, response.Count);
        Assert.Equal("Tag1", response[0].Name);
        Assert.Equal(2, response[0].Count);
        Assert.Equal("Tag2", response[1].Name);
        Assert.Equal(3, response[1].Count);
    }
}
