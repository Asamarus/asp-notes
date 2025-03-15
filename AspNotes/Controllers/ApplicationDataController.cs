using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Mime;

namespace AspNotes.Controllers;

/// <summary>
/// Controller for application data.
/// </summary>
[Route("api/application-data")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class ApplicationDataController(
    ISectionsService sectionsService,
    IOptions<AllNotesSection> allNotesSection)
    : ControllerBase
{
    /// <summary>
    /// Gets the application data.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The application data response.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationDataResponse))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var sections = await sectionsService.GetSections(cancellationToken);

        return Ok(new ApplicationDataResponse
        {
            AllNotesSection = allNotesSection.Value,
            Sections = sections.Select(SectionsItemResponse.FromEntity),
        });
    }
}
