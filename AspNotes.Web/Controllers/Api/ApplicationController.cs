using Ardalis.GuardClauses;
using AspNotes.Web.Helpers;
using AspNotes.Web.Models;
using AspNotes.Web.Models.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNotes.Web.Controllers.Api;

[Route("api/application")]
[ApiController]
[Produces("application/json")]
public class ApplicationController(IOptions<AllNotesSection> allNotesSection) : ControllerBase
{
    /// <summary>
    /// Retrieves the initial data for the application
    /// </summary>
    /// <remarks>Returns a response containing the title and some data</remarks>
    /// <response code="200">Initial data retrieved successfully</response>
    [HttpPost("getInitialData")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InitialDataResponse))]
    public IActionResult GetInitialData()
    {
        var user = User.GetCurrentUser();
        Guard.Against.Null(user);

        return Ok(new InitialDataResponse
        {
            AllNotesSection = allNotesSection.Value
        });
    }
}
