using AspNotes.Core.Book;
using AspNotes.Core.Book.Models;
using AspNotes.Core.Section;
using AspNotes.Web.Controllers.Api;
using AspNotes.Web.Models.Books;
using AspNotes.Web.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AspNotes.Web.Tests.Controllers.Api;

public class BooksControllerTests
{
    private readonly Mock<IBooksService> _booksServiceMock;
    private readonly Mock<ISectionsService> _sectionsServiceMock;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _booksServiceMock = new Mock<IBooksService>();
        _sectionsServiceMock = new Mock<ISectionsService>();
        _controller = new BooksController(_booksServiceMock.Object, _sectionsServiceMock.Object);
    }

    [Fact]
    public async Task GetBooksList_ReturnsOkResult_WithListOfBooks()
    {
        // Arrange
        var request = new GetBooksListRequest { Section = "Fiction" };
        var books = new List<BooksServiceGetBooksResultItem>
            {
                new() { Id = 1, Name = "Book1", Count = 1 },
                new() { Id = 2, Name = "Book1", Count = 2 },
                new() { Id = 3, Name = "Book2", Count = 1 }
            };

        _sectionsServiceMock.Setup(s => s.IsSectionNameValid(request.Section)).ReturnsAsync(true);
        _booksServiceMock.Setup(s => s.GetBooks(request.Section)).ReturnsAsync(books);

        // Act
        var result = await _controller.GetBooksList(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<BookItemResponse>>(okResult.Value);
        Assert.Equal(2, response.Count);
        Assert.Equal("Book1", response[0].Name);
        Assert.Equal(3, response[0].Count);
        Assert.Equal("Book2", response[1].Name);
        Assert.Equal(1, response[1].Count);
    }

    [Fact]
    public async Task GetBooksList_ReturnsBadRequest_WhenSectionNameIsInvalid()
    {
        // Arrange
        var request = new GetBooksListRequest { Section = "InvalidSection" };

        _sectionsServiceMock.Setup(s => s.IsSectionNameValid(request.Section)).ReturnsAsync(false);

        // Act
        var result = await _controller.GetBooksList(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal($"Section '{request.Section}' is not valid!", errorResponse.Message);
    }

    [Fact]
    public async Task AutocompleteBooks_ReturnsOkResult_WithListOfBookNames()
    {
        // Arrange
        var request = new AutocompleteBooksRequest { SearchTerm = "Bo", Section = "Fiction" };
        var books = new List<BooksServiceAutoCompleteResultItem>
            {
                new() { Id = "1", Name = "Book1" },
                new() { Id = "2", Name = "Book2" },
                new() { Id = "3", Name = "Book1" }
            };

        _sectionsServiceMock.Setup(s => s.IsSectionNameValid(request.Section)).ReturnsAsync(true);
        _booksServiceMock.Setup(s => s.Autocomplete(request.SearchTerm, request.Section)).ReturnsAsync(books);

        // Act
        var result = await _controller.AutocompleteBooks(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<string>>(okResult.Value);
        Assert.Equal(2, response.Count);
        Assert.Contains("Book1", response);
        Assert.Contains("Book2", response);
    }

    [Fact]
    public async Task AutocompleteBooks_ReturnsBadRequest_WhenSectionNameIsInvalid()
    {
        // Arrange
        var request = new AutocompleteBooksRequest { SearchTerm = "Bo", Section = "InvalidSection" };

        _sectionsServiceMock.Setup(s => s.IsSectionNameValid(request.Section)).ReturnsAsync(false);

        // Act
        var result = await _controller.AutocompleteBooks(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal($"Section '{request.Section}' is not valid!", errorResponse.Message);
    }
}
