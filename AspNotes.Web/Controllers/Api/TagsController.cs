using AspNotes.Core.Section;
using AspNotes.Core.Tag;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Tags;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Controllers.Api;

[Route("api/tags")]
[ApiController]
[Produces("application/json")]
public class TagsController(ITagsService tagsService, ISectionsService sectionsService) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of tags based on the provided section.
    /// </summary>
    /// <param name="request">The request containing the section to filter tags.</param>
    /// <returns>A list of tags that match the request criteria.</returns>
    /// <response code="200">Returns the list of tags.</response>
    /// <response code="400">If the section name is invalid.</response>
    [HttpPost("getList")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TagItemResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetTagsList(GetTagsListRequest request)
    {
        if (!string.IsNullOrEmpty(request.Section))
        {
            var isSectionNameValid = await sectionsService.IsSectionNameValid(request.Section);

            if (!isSectionNameValid)
                return BadRequest(new ErrorResponse { Message = $"Section '{request.Section}' is not valid!" });
        }

        var books = await tagsService.GetTags(request.Section);

        var response = books
            .GroupBy(book => book.Name)
            .Select(group => new TagItemResponse
            {
                Name = group.Key,
                Count = group.Sum(book => book.Count)
            })
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Provides tag name suggestions based on the input term and optional section.
    /// </summary>
    /// <param name="request">The autocomplete request containing the search term and optional section.</param>
    /// <returns>A list of tag names that match the autocomplete criteria.</returns>
    /// <response code="200">Returns the list of suggested tag names.</response>
    /// <response code="400">If the section name is invalid.</response>
    [HttpPost("autocomplete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> AutocompleteTags(AutocompleteTagsRequest request)
    {
        if (!string.IsNullOrEmpty(request.Section))
        {
            var isSectionNameValid = await sectionsService.IsSectionNameValid(request.Section);

            if (!isSectionNameValid)
                return BadRequest(new ErrorResponse { Message = $"Section '{request.Section}' is not valid!" });
        }

        var books = await tagsService.Autocomplete(request.SearchTerm, request.Section);

        var response = books.Select(x => x.Name).Distinct().ToList();

        return Ok(response);
    }
}