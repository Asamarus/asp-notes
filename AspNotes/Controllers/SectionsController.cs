using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace AspNotes.Controllers;

/// <summary>
/// Controller for managing sections.
/// </summary>
[Route("api/sections")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class SectionsController(ISectionsService sectionsService)
    : ControllerBase
{
    /// <summary>
    /// Gets all sections.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of sections.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SectionsItemResponse>))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sections = await sectionsService.GetSections(cancellationToken);

        return Ok(sections.Select(SectionsItemResponse.FromEntity));
    }

    /// <summary>
    /// Creates a new section.
    /// </summary>
    /// <param name="request">The section creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created section.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<SectionsItemResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Post([FromBody] SectionsCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await sectionsService.IsSectionNameUnique(request.Name, cancellationToken))
        {
            return ValidationProblem(title: "Section name is not unique!");
        }

        await sectionsService.CreateSection(request.Name, request.DisplayName, request.Color, cancellationToken);

        var sections = await sectionsService.GetSections(cancellationToken);

        return CreatedAtAction(nameof(Get), sections.Select(SectionsItemResponse.FromEntity));
    }

    /// <summary>
    /// Updates an existing section.
    /// </summary>
    /// <param name="id">The section ID.</param>
    /// <param name="request">The section update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated section.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SectionsItemResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Put(long id, [FromBody] SectionsUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!await sectionsService.IsSectionIdPresent(id, cancellationToken))
        {
            return NotFound();
        }

        await sectionsService.UpdateSection(id, request.DisplayName, request.Color, cancellationToken);
        var sections = await sectionsService.GetSections(cancellationToken);
        return Ok(sections.Select(SectionsItemResponse.FromEntity));
    }

    /// <summary>
    /// Deletes an existing section.
    /// </summary>
    /// <param name="id">The section ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The remaining sections.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SectionsItemResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        if (!await sectionsService.IsSectionIdPresent(id, cancellationToken))
        {
            return NotFound();
        }

        if (await sectionsService.IsSectionHavingNotes(id, cancellationToken))
        {
            return ValidationProblem(title: "Section has notes!");
        }

        await sectionsService.DeleteSection(id, cancellationToken);
        var sections = await sectionsService.GetSections(cancellationToken);
        return Ok(sections.Select(SectionsItemResponse.FromEntity));
    }

    /// <summary>
    /// Reorders sections.
    /// </summary>
    /// <param name="request">The reorder request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reordered sections.</returns>
    [HttpPut("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SectionsItemResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Reorder([FromBody] SectionsReorderRequest request, CancellationToken cancellationToken)
    {
        if (!await sectionsService.ReorderSections(request.Ids, cancellationToken))
        {
            return ValidationProblem(title: "Sections are not reordered!");
        }

        var sections = await sectionsService.GetSections(cancellationToken);
        return Ok(sections.Select(SectionsItemResponse.FromEntity));
    }
}