using AspNotes.Core.Common;
using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Note;
using AspNotes.Core.Note.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using System.Diagnostics;

namespace AspNotes.Web.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class DevController(IWebHostEnvironment env, NotesDbContext context, QueryFactory db) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("test")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Get()
    {
        if (!env.IsDevelopment())
        {
            return NotFound();
        }

        var notesService = HttpContext.RequestServices.GetService<NotesService>();

        Debug.WriteLine("Test");


        return Ok("Ok");
    }
}
