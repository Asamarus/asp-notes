using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace AspNotes.Controllers;

/// <summary>
/// Controller for books.
/// </summary>
[Route("api/books")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class BooksController(IBooksService booksService, ISectionsService sectionsService)
    : ControllerBase
{
    /// <summary>
    /// Retrieves a list of books based on the provided section.
    /// </summary>
    /// <param name="section">The section to filter books by (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of sections.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ItemNameCountResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Get([FromQuery] string? section, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(section))
        {
            var isSectionNameValid = await sectionsService.IsSectionNameValid(section, cancellationToken);

            if (!isSectionNameValid)
            {
                return ValidationProblem(title: $"Section '{section}' is not valid!");
            }
        }

        var books = await booksService.GetBooks(section, cancellationToken);

        var response = books
            .GroupBy(book => book.Name)
            .Select(group => new ItemNameCountResponse
            {
                Name = group.Key,
                Count = group.Sum(book => book.Count),
            })
            .ToList();

        return Ok(response);
    }
}
