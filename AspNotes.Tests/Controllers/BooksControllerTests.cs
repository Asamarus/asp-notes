using AspNotes.Controllers;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AspNotes.Tests.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBooksService> booksServiceMock;
    private readonly Mock<ISectionsService> sectionsServiceMock;
    private readonly BooksController controller;

    public BooksControllerTests()
    {
        booksServiceMock = new Mock<IBooksService>();
        sectionsServiceMock = new Mock<ISectionsService>();
        controller = new BooksController(booksServiceMock.Object, sectionsServiceMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnOkResult_WhenSectionIsValid()
    {
        // Arrange
        var section = "validSection";
        var books = new List<(long Id, string Name, int Count)>
        {
            (1, "Book1", 2),
            (2, "Book2", 3)
        };
        sectionsServiceMock.Setup(s => s.IsSectionNameValid(section, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        booksServiceMock.Setup(b => b.GetBooks(section, It.IsAny<CancellationToken>())).ReturnsAsync(books);

        // Act
        var result = await controller.Get(section, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<ItemNameCountResponse>>(okResult.Value);
        Assert.Equal(2, response.Count);
        Assert.Equal("Book1", response[0].Name);
        Assert.Equal(2, response[0].Count);
        Assert.Equal("Book2", response[1].Name);
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
        var books = new List<(long Id, string Name, int Count)>
        {
            (1, "Book1", 2),
            (2, "Book2", 3)
        };
        booksServiceMock.Setup(b => b.GetBooks(section, It.IsAny<CancellationToken>())).ReturnsAsync(books);

        // Act
        var result = await controller.Get(section, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<ItemNameCountResponse>>(okResult.Value);
        Assert.Equal(2, response.Count);
        Assert.Equal("Book1", response[0].Name);
        Assert.Equal(2, response[0].Count);
        Assert.Equal("Book2", response[1].Name);
        Assert.Equal(3, response[1].Count);
    }
}
