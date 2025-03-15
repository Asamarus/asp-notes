using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace AspNotes.Controllers;

/// <summary>
/// Controller for tags.
/// </summary>
[Route("api/tags")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TagsController(ITagsService tagsService, ISectionsService sectionsService)
    : ControllerBase
{
    /// <summary>
    /// Retrieves a list of tags based on the provided section.
    /// </summary>
    /// <param name="section">The section to filter tags by (optional).</param>
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

        var tags = await tagsService.GetTags(section, cancellationToken);

        var response = tags
            .GroupBy(tag => tag.Name)
            .Select(group => new ItemNameCountResponse
            {
                Name = group.Key,
                Count = group.Sum(tag => tag.Count),
            })
            .ToList();

        return Ok(response);
    }
}