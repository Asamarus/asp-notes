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

        //await notesService.CreateNote(new NoteDto
        //{
        //    Title = "Note 1",
        //    Content = "This is some string",
        //    Section = "section1"
        //});

        //await notesService.CreateNote(new NoteDto
        //{
        //    Title = "Note 2",
        //    Content = "This is another string",
        //    Section = "section1"
        //});

        //await notesService.CreateNote(new NoteDto
        //{
        //    Title = "Note 3",
        //    Content = "Something",
        //    Section = "section1"
        //});

        //await notesService.UpdateNote(new NoteDto
        //{
        //    Id = 3,
        //    Title = "Note 3",
        //    Content = "Something too",
        //    Section = "section1"
        //});


        var n = new NotesTable("n");

        var notesQuery = db.Query()
            .Select(n.Id, n.Title, n.Content)
            .From(n.GetFormattedTableName())
            .Where(n.Section, "section1");

        var result = await SearchHelper.FullTextSearch(new Core.Common.Models.SearchHelperFullTextSearchRequest
        {
            Query = notesQuery,
            FtsSearchColumns = NoteFtsSettings.FtsSearchColumns,
            SearchTerm = "somelong search",
            FtsTableName = NoteFtsSettings.FtsTableName,
            MainContentTableName = NoteFtsSettings.MainContentTableName,
            FtsPrimaryKey = NoteFtsSettings.FtsPrimaryKey,
            MainContentTablePrimaryKey = NoteFtsSettings.MainContentTablePrimaryKey
        });

        var notes = await result.Query.GetAsync();

        //var allNotes = await context.Notes.ToListAsync();

        //System.Diagnostics.Debug.WriteLine("Notes length: " + allNotes.Count);

        //var allNotes2 = db.Query("Notes").Get();
        //var allNotes2 = db.Query("Notes").Get<NoteEntity>();

        return Ok("Ok");
    }
}
