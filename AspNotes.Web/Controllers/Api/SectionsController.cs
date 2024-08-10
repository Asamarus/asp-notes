using AspNotes.Core.Section;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Sections;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Controllers.Api;

[Route("api/sections")]
[ApiController]
[Produces("application/json")]
public class SectionsController(ISectionsService sectionsService) : ControllerBase
{
    /// <summary>
    /// Retrieves all sections
    /// </summary>
    /// <remarks>Returns a list of all sections</remarks>
    /// <response code="200">Sections retrieved successfully</response>
    [HttpPost("getList")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionsResponse))]
    public async Task<IActionResult> GetSectionsList()
    {
        var sections = await sectionsService.GetSections();

        var response = new SectionsResponse
        {
            Sections = sections.Select(x => new SectionItemResponse(x)).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a new section with the provided details.
    /// </summary>
    /// <param name="request">The details of the section to create.</param>
    /// <returns>A response indicating the result of the creation operation.</returns>
    /// <response code="200">If the section is created successfully. The response includes the details of all sections.</response>
    /// <response code="400">If the section name is not unique.</response>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateSection(CreateSectionRequest request)
    {
        if (!await sectionsService.IsSectionNameUnique(request.Name))
            return BadRequest(new ErrorResponse { Message = "Section name is not unique!" });

        var newSectionId = await sectionsService.CreateSection(request.Name, request.DisplayName, request.Color);

        var sections = await sectionsService.GetSections();

        var response = new SectionsResponse
        {
            Message = "Section is created successfully!",
            Sections = sections.Select(x => new SectionItemResponse(x)).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Updates an existing section with the provided details.
    /// </summary>
    /// <param name="request">The details of the section to update.</param>
    /// <returns>A response indicating the result of the update operation.</returns>
    /// <response code="200">If the section is updated successfully. The response includes the updated list of sections.</response>
    /// <response code="400">If the section with the provided ID is not found or the update operation fails.</response>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateSection(UpdateSectionRequest request)
    {
        if (!await sectionsService.IsSectionIdPresent(request.Id))
            return BadRequest(new ErrorResponse { Message = $"Section with Id '{request.Id}' is not found!" });

        if (!await sectionsService.UpdateSection(request.Id, request.DisplayName, request.Color))
            return BadRequest(new ErrorResponse { Message = "Section is not updated!" });

        var sections = await sectionsService.GetSections();

        var response = new SectionsResponse
        {
            Message = "Section is updated successfully!",
            Sections = sections.Select(x => new SectionItemResponse(x)).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Deletes a section based on the provided section ID.
    /// </summary>
    /// <param name="request">The request containing the ID of the section to delete.</param>
    /// <returns>A response indicating the result of the deletion operation.</returns>
    /// <response code="200">If the section is deleted successfully. The response includes the updated list of sections.</response>
    /// <response code="400">If the section with the provided ID is not found or the deletion operation fails.</response>
    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> DeleteSection(DeleteSectionRequest request)
    {
        if (!await sectionsService.IsSectionIdPresent(request.Id))
            return BadRequest(new ErrorResponse { Message = $"Section with Id '{request.Id}' is not found!" });

        if (!await sectionsService.DeleteSection(request.Id))
            return BadRequest(new ErrorResponse { Message = "Section is not deleted!" });

        var sections = await sectionsService.GetSections();

        var response = new SectionsResponse
        {
            Message = "Section is deleted successfully!",
            Sections = sections.Select(x => new SectionItemResponse(x)).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Reorders sections based on the provided order.
    /// </summary>
    /// <param name="request">The request containing the new order of sections.</param>
    /// <returns>A response indicating the result of the reorder operation.</returns>
    /// <response code="200">If the sections are reordered successfully. The response includes the updated list of sections.</response>
    /// <response code="400">If the reorder operation fails.</response>
    [HttpPost("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> ReorderSections(ReorderSectionsRequest request)
    {
        if (!await sectionsService.ReorderSections(request.Ids))
            return BadRequest(new ErrorResponse { Message = "Sections are not reordered!" });

        var sections = await sectionsService.GetSections();

        var response = new SectionsResponse
        {
            Message = "Sections are reordered successfully!",
            Sections = sections.Select(x => new SectionItemResponse(x)).ToList()
        };

        return Ok(response);
    }
}