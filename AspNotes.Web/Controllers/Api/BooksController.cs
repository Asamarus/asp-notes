using AspNotes.Core.Book;
using AspNotes.Core.Section;
using AspNotes.Web.Models.Books;
using AspNotes.Web.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Controllers.Api;

[Route("api/books")]
[ApiController]
[Produces("application/json")]
public class BooksController(IBooksService booksService, ISectionsService sectionsService) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of books based on the provided section.
    /// </summary>
    /// <param name="request">The request containing the section to filter books.</param>
    /// <returns>A list of books that match the request criteria.</returns>
    /// <response code="200">Returns the list of books.</response>
    /// <response code="400">If the section name is invalid.</response>
    [HttpPost("getList")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BookItemResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetBooksList(GetBooksListRequest request)
    {
        if (!string.IsNullOrEmpty(request.Section))
        {
            var isSectionNameValid = await sectionsService.IsSectionNameValid(request.Section);

            if (!isSectionNameValid)
                return BadRequest(new ErrorResponse { Message = $"Section '{request.Section}' is not valid!" });
        }

        var books = await booksService.GetBooks(request.Section);

        var response = books
            .GroupBy(book => book.Name)
            .Select(group => new BookItemResponse
            {
                Name = group.Key,
                Count = group.Sum(book => book.Count)
            })
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Provides book name suggestions based on the input term and optional section.
    /// </summary>
    /// <param name="request">The autocomplete request containing the search term and optional section.</param>
    /// <returns>A list of book names that match the autocomplete criteria.</returns>
    /// <response code="200">Returns the list of suggested book names.</response>
    /// <response code="400">If the section name is invalid.</response>
    [HttpPost("autocomplete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> AutocompleteBooks(AutocompleteBooksRequest request)
    {
        if (!string.IsNullOrEmpty(request.Section))
        {
            var isSectionNameValid = await sectionsService.IsSectionNameValid(request.Section);

            if (!isSectionNameValid)
                return BadRequest(new ErrorResponse { Message = $"Section '{request.Section}' is not valid!" });
        }

        var books = await booksService.Autocomplete(request.SearchTerm, request.Section);

        var response = books.Select(x => x.Name).Distinct().ToList();

        return Ok(response);
    }
}