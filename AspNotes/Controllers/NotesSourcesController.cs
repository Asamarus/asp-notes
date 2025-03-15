using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace AspNotes.Controllers;

/// <summary>
/// Controller for managing notes sources.
/// </summary>
[Route("api/notes/{noteId}/sources")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class NotesSourcesController(
    IAppDbContext dbContext,
    IUrlMetadataHelper urlMetadataHelper)
    : ControllerBase
{
    /// <summary>
    /// Adds a new source to a note.
    /// </summary>
    /// <param name="noteId">The note ID.</param>
    /// <param name="request">The note source request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of note's sources.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(List<NotesSource>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddNoteSource(
        long noteId,
        [FromBody] NotesSourceCreateRequest request,
        CancellationToken cancellationToken)
    {
        var noteEntity = await dbContext.Notes.FindAsync([noteId], cancellationToken);

        if (noteEntity == null)
        {
            return NotFound();
        }

        var urlMetadata = await urlMetadataHelper.GetUrlMetadata(request.Link, cancellationToken);

        noteEntity.Sources.Add(new NotesSource
        {
            Id = Guid.NewGuid().ToString(),
            Link = request.Link,
            Title = urlMetadata.Title,
            Description = urlMetadata.Description?.Length > 100
                    ? urlMetadata.Description[..100] + "..."
                    : urlMetadata.Description,
            Image = urlMetadata.Image,
            ShowImage = !string.IsNullOrEmpty(urlMetadata.Image),
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(null, noteEntity.Sources);
    }

    /// <summary>
    /// Updates an existing source of a note.
    /// </summary>
    /// <param name="noteId">The note ID.</param>
    /// <param name="id">The note source ID.</param>
    /// <param name="request">The note source update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of note's sources.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NotesSource>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateNoteSource(
        long noteId,
        string id,
        [FromBody] NotesSourceUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var noteEntity = await dbContext.Notes.FindAsync([noteId], cancellationToken);

        if (noteEntity == null)
        {
            return NotFound();
        }

        var source = noteEntity.Sources.FirstOrDefault(x => x.Id == id);

        if (source == null)
        {
            return NotFound();
        }

        source.Link = request.Link;
        source.Title = request.Title;
        source.Description = request.Description;
        source.ShowImage = request.ShowImage;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(noteEntity.Sources);
    }

    /// <summary>
    /// Removes a source from a note.
    /// </summary>
    /// <param name="noteId">The note ID.</param>
    /// <param name="id">The note source ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of note's sources.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NotesSource>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long noteId,
        string id,
        CancellationToken cancellationToken)
    {
        var noteEntity = await dbContext.Notes.FindAsync([noteId], cancellationToken);

        if (noteEntity == null)
        {
            return NotFound();
        }

        var source = noteEntity.Sources.FirstOrDefault(x => x.Id == id);

        if (source == null)
        {
            return NotFound();
        }

        noteEntity.Sources.Remove(source);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(noteEntity.Sources);
    }

    /// <summary>
    /// Reorders the sources of a note.
    /// </summary>
    /// <param name="noteId">The note ID.</param>
    /// <param name="request">The reorder request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of note's sources.</returns>
    [HttpPut("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NotesSource>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Reorder(
        long noteId,
        [FromBody] NotesSourcesReorderRequest request,
        CancellationToken cancellationToken)
    {
        var noteEntity = await dbContext.Notes.FindAsync([noteId], cancellationToken);

        if (noteEntity == null)
        {
            return NotFound();
        }

        var reorderedSources = new List<NotesSource>();

        foreach (var sourceId in request.SourceIds)
        {
            var source = noteEntity.Sources.FirstOrDefault(x => x.Id == sourceId);

            if (source == null)
            {
                return ValidationProblem(title: $"Source with id '{sourceId}' is not found!");
            }

            reorderedSources.Add(source);
        }

        noteEntity.Sources = reorderedSources;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(noteEntity.Sources);
    }
}
