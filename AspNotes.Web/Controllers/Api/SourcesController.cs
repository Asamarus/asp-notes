using AspNotes.Core.Note;
using AspNotes.Core.Note.Models;
using AspNotes.Web.Helpers;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Sources;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Controllers.Api;

[Route("api/sources")]
[ApiController]
[Produces("application/json")]
public class SourcesController(INotesService notesService, IUrlMetadataHelper urlMetadataHelper) : ControllerBase
{
    /// <summary>
    /// Adds a new source to a note.
    /// </summary>
    /// <param name="request">The request containing the note ID and source URL to add.</param>
    /// <returns>A response indicating success or failure, along with the updated list of sources for the note.</returns>
    /// <response code="200">Returns the updated list of sources for the note.</response>
    /// <response code="400">If the note is not found or the request is invalid.</response>
    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SourcesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> AddNoteSource(AddNoteSourceRequest request)
    {
        var note = await notesService.GetNoteById(request.NoteId);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.NoteId}' is not found!" });

        var urlMetadata = await urlMetadataHelper.GetUrlMetadata(request.Link);

        note.Sources.Add(new NoteSource
        {
            Id = Guid.NewGuid().ToString(),
            Link = request.Link,
            Title = urlMetadata.Title,
            Description = urlMetadata.Description,
            Image = urlMetadata.Image,
            ShowImage = !string.IsNullOrEmpty(urlMetadata.Image)
        });

        var updatedNote = await notesService.UpdateNoteSources(request.NoteId, note.Sources);

        var response = new SourcesResponse
        {
            Message = "New note source is created successfully!",
            Sources = updatedNote.Sources.Select(x => new SourceItemResponse(x)).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Updates an existing source of a note.
    /// </summary>
    /// <param name="request">The request containing the note ID, source ID, and new source details.</param>
    /// <returns>A response indicating success or failure, along with the updated list of sources for the note.</returns>
    /// <response code="200">Returns the updated list of sources for the note.</response>
    /// <response code="400">If the note or source is not found or the request is invalid.</response>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SourcesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateNoteSource(UpdateNoteSourceRequest request)
    {
        var note = await notesService.GetNoteById(request.NoteId);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.NoteId}' is not found!" });

        var source = note.Sources.FirstOrDefault(x => x.Id == request.SourceId);

        if (source == null)
            return BadRequest(new ErrorResponse { Message = $"Source with id '{request.SourceId}' is not found!" });

        source.Link = request.Link;
        source.Title = request.Title;
        source.Description = request.Description;
        source.Image = request.Image;
        source.ShowImage = request.ShowImage;

        var updatedNote = await notesService.UpdateNoteSources(request.NoteId, note.Sources);

        var response = new SourcesResponse
        {
            Message = "New note source is updated successfully!",
            Sources = updatedNote.Sources.Select(x => new SourceItemResponse(x)).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Removes a source from a note.
    /// </summary>
    /// <param name="request">The request containing the note ID and source ID to remove.</param>
    /// <returns>A response indicating success or failure, along with the updated list of sources for the note.</returns>
    /// <response code="200">Returns the updated list of sources for the note.</response>
    /// <response code="400">If the note or source is not found or the request is invalid.</response>
    [HttpPost("remove")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SourcesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> RemoveNoteSource(RemoveNoteSourceRequest request)
    {
        var note = await notesService.GetNoteById(request.NoteId);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.NoteId}' is not found!" });

        var source = note.Sources.FirstOrDefault(x => x.Id == request.SourceId);

        if (source == null)
            return BadRequest(new ErrorResponse { Message = $"Source with id '{request.SourceId}' is not found!" });

        note.Sources.Remove(source);

        var updatedNote = await notesService.UpdateNoteSources(request.NoteId, note.Sources);

        var response = new SourcesResponse
        {
            Message = "Note source is removed successfully!",
            Sources = updatedNote.Sources.Select(x => new SourceItemResponse(x)).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Reorders the sources of a note.
    /// </summary>
    /// <param name="request">The request containing the note ID and a list of source IDs in the new order.</param>
    /// <returns>A response indicating success or failure, along with the reordered list of sources for the note.</returns>
    /// <response code="200">Returns the reordered list of sources for the note.</response>
    /// <response code="400">If the note is not found, a source is not found, or the request is invalid.</response>
    [HttpPost("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SourcesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> ReorderNoteSources(ReorderNoteSourcesRequest request)
    {
        var note = await notesService.GetNoteById(request.NoteId);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.NoteId}' is not found!" });

        var reorderedSources = new List<NoteSource>();

        foreach (var sourceId in request.SourceIds)
        {
            var source = note.Sources.FirstOrDefault(x => x.Id == sourceId);

            if (source == null)
                return BadRequest(new ErrorResponse { Message = $"Source with id '{sourceId}' is not found!" });

            reorderedSources.Add(source);
        }

        var updatedNote = await notesService.UpdateNoteSources(request.NoteId, reorderedSources);

        var response = new SourcesResponse
        {
            Message = "Note sources are reordered successfully!",
            Sources = updatedNote.Sources.Select(x => new SourceItemResponse(x)).ToList()
        };

        return Ok(response);
    }
}