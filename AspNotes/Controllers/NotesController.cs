using AspNotes.Common;
using AspNotes.Entities;
using AspNotes.Helpers;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace AspNotes.Controllers;

/// <summary>
/// Controller for managing notes.
/// </summary>
[Route("api/notes")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class NotesController(
    IAppDbContext dbContext,
    ISectionsService sectionsService,
    IBooksService booksService,
    ITagsService tagsService,
    ILogger<NotesController> logger)
    : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of notes based on the specified request parameters.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of notes.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResponse<NotesItemResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Get([FromQuery] NotesGetRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.Notes.AsNoTracking();

        var keywords = new HashSet<string>();
        var foundWholePhrase = false;

        if (!string.IsNullOrEmpty(request.Section))
        {
            var isSectionNameValid = await sectionsService.IsSectionNameValid(request.Section, cancellationToken);

            if (!isSectionNameValid)
            {
                return ValidationProblem(title: $"Section '{request.Section}' is not valid!");
            }

            query = query.Where(note => note.Section == request.Section);
        }

        if (!string.IsNullOrEmpty(request.Book))
        {
            query = query.Where(note => note.Book == request.Book);
        }

        if (request.Tags.Count > 0)
        {
            var predicate = PredicateBuilder.False<NoteEntity>();

            foreach (var tag in request.Tags)
            {
                predicate = predicate.Or(n => EF.Functions.Like(n.Tags, $"%[{tag}]%"));
            }

            query = query.Where(predicate);
        }

        if (request.WithoutTags)
        {
            query = query.Where(note => string.IsNullOrWhiteSpace(note.Tags));
        }

        if (request.WithoutBook)
        {
            query = query.Where(note => string.IsNullOrEmpty(note.Book));
        }

        if (request.FromDate.HasValue)
        {
            var fromDate = request.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(note => note.CreatedAt >= fromDate);
        }

        if (request.ToDate.HasValue)
        {
            var toDate = request.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(note => note.CreatedAt <= toDate);
        }

        var hasSearchTerm = !string.IsNullOrEmpty(request.SearchTerm) && request.SearchTerm.Length > 2;

        if (hasSearchTerm)
        {
            var searchResult = await NotesSearchHelper.FullTextSearch(query, request.SearchTerm!.ToLower(), cancellationToken);

            query = searchResult.Query;
            keywords = searchResult.Keywords;
            foundWholePhrase = searchResult.FoundWholePhrase;
        }

        if (!hasSearchTerm)
        {
            if (request.InRandomOrder)
            {
                query = query.OrderBy(n => EF.Functions.Random());
            }
            else
            {
                query = query.OrderByDescending(note => note.CreatedAt);
            }
        }

        var resultsPerPage = 50;

        var totalItems = await query.CountAsync(cancellationToken);

        var notes = await query
            .Skip((request.Page - 1) * resultsPerPage)
            .Take(resultsPerPage)
            .ToListAsync(cancellationToken);

        if (hasSearchTerm)
        {
            notes = [.. notes.Select(note =>
            {
                note.Preview = SearchHelper.GetSearchSnippet(
                    keywords,
                    SearchHelper.GetSearchIndex(note.Content ?? string.Empty),
                    10,
                    foundWholePhrase);
                return note;
            })];
        }

        return Ok(new PaginatedResponse<NotesItemResponse>
        {
            Total = totalItems,
            Count = notes.Count,
            LastPage = (int)Math.Ceiling((double)totalItems / resultsPerPage),
            CanLoadMore = totalItems > request.Page * resultsPerPage,
            Page = request.Page,
            Data = notes.Select(NotesItemResponse.FromEntity),
            SearchTerm = request.SearchTerm,
            Keywords = keywords,
            FoundWholePhrase = foundWholePhrase,
        });
    }

    /// <summary>
    /// Gets a note by its ID.
    /// </summary>
    /// <param name="id">The note ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The note with the specified ID.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NotesItemResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
    {
        var noteEntity = await dbContext.Notes.FindAsync([id], cancellationToken);

        if (noteEntity == null)
        {
            return NotFound();
        }

        return Ok(NotesItemResponse.FromEntity(noteEntity));
    }

    /// <summary>
    /// Creates a new note.
    /// </summary>
    /// <param name="request">The note creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created note.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NotesItemResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Post([FromBody] NotesCreateRequest request, CancellationToken cancellationToken)
    {
        var isSectionNameValid = await sectionsService.IsSectionNameValid(request.Section, cancellationToken);

        if (!isSectionNameValid)
        {
            return ValidationProblem(title: $"Section '{request.Section}' is not valid!");
        }

        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var noteEntity = new NoteEntity
            {
                Section = request.Section,
            };

            dbContext.Notes.Add(noteEntity);

            await dbContext.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrEmpty(request.Book)
                && !await booksService.UpdateNoteBook(noteEntity.Id, request.Book, cancellationToken, transaction))
            {
                return Problem(title: "Book update failed!");
            }

            await transaction.CommitAsync(cancellationToken);

            return CreatedAtAction(nameof(Get), NotesItemResponse.FromEntity(noteEntity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the note: {ErrorMessage}", ex.Message);
            await transaction.RollbackAsync(cancellationToken);
            return Problem(title: "An error occurred while creating the note.");
        }
    }

    /// <summary>
    /// Updates an existing note.
    /// </summary>
    /// <param name="id">The note ID.</param>
    /// <param name="request">The note update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NotesItemResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Patch(long id, [FromBody] NotesPatchRequest request, CancellationToken cancellationToken)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var noteEntity = await dbContext.Notes.FindAsync([id], cancellationToken);

            if (noteEntity == null)
            {
                return NotFound();
            }

            if (request.Section != null && noteEntity.Section != request.Section)
            {
                var isSectionNameValid = await sectionsService.IsSectionNameValid(request.Section, cancellationToken);

                if (!isSectionNameValid)
                {
                    return ValidationProblem(title: $"Section '{request.Section}' is not valid!");
                }

                var book = noteEntity.Book;
                var tags = new List<string>(noteEntity.TagsList).ToHashSet();

                // Update book and tags to empty values to clear book and tags for current section
                if (!await booksService.UpdateNoteBook(noteEntity.Id, string.Empty, cancellationToken, transaction))
                {
                    return Problem(title: "Book update failed!");
                }

                if (!await tagsService.UpdateNoteTags(noteEntity.Id, [], cancellationToken, transaction))
                {
                    return Problem(title: "Tags update failed!");
                }

                noteEntity.Section = request.Section;

                // Update book and tags back to original values
                if (!await booksService.UpdateNoteBook(noteEntity.Id, book ?? string.Empty, cancellationToken, transaction))
                {
                    return Problem(title: "Book update failed!");
                }

                if (!await tagsService.UpdateNoteTags(noteEntity.Id, tags, cancellationToken, transaction))
                {
                    return Problem(title: "Tags update failed!");
                }
            }

            if (request.Book != null &&
                noteEntity.Book != request.Book &&
                !await booksService.UpdateNoteBook(noteEntity.Id, request.Book, cancellationToken, transaction))
            {
                return Problem(title: "Book update failed!");
            }

            if (request.Tags != null)
            {
                var requestTags = request.Tags.ToHashSet();
                var noteTags = noteEntity.TagsList.ToHashSet();

                if (!requestTags.SetEquals(noteTags) &&
                    !await tagsService.UpdateNoteTags(noteEntity.Id, requestTags, cancellationToken, transaction))
                {
                    return Problem(title: "Tags update failed!");
                }
            }

            if (request.Title != null && request.Title != noteEntity.Title)
            {
                noteEntity.Title = request.Title;
                noteEntity.TitleSearchIndex = string.IsNullOrEmpty(request.Title) ? null : SearchHelper.GetSearchIndex(request.Title);
            }

            if (request.Content != null && request.Content != noteEntity.Content)
            {
                noteEntity.Content = request.Content;
                noteEntity.ContentSearchIndex = string.IsNullOrEmpty(request.Content) ? null : SearchHelper.GetSearchIndex(request.Content);
                noteEntity.Preview = string.IsNullOrEmpty(request.Content) ? null : SearchHelper.GetSearchIndex(request.Content, false, 100);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            noteEntity = await dbContext.Notes.FindAsync([id], cancellationToken);

            return Ok(NotesItemResponse.FromEntity(noteEntity!));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the note: {ErrorMessage}", ex.Message);
            await transaction.RollbackAsync(cancellationToken);
            return Problem(title: "An error occurred while updating the note.");
        }
    }

    /// <summary>
    /// Deletes a note by its ID.
    /// </summary>
    /// <param name="id">The note ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var noteEntity = await dbContext.Notes.FindAsync([id], cancellationToken);

            if (noteEntity == null)
            {
                return NotFound();
            }

            if (!await booksService.UpdateNoteBook(noteEntity.Id, string.Empty, cancellationToken, transaction))
            {
                return Problem(title: "Book update failed!");
            }

            if (!await tagsService.UpdateNoteTags(noteEntity.Id, [], cancellationToken, transaction))
            {
                return Problem(title: "Tags update failed!");
            }

            dbContext.Notes.Remove(noteEntity);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting the note: {ErrorMessage}", ex.Message);
            await transaction.RollbackAsync(cancellationToken);
            return Problem(title: "An error occurred while deleting the note.");
        }
    }

    /// <summary>
    /// Gets autocomplete suggestions for notes, books, and tags.
    /// </summary>
    /// <param name="request">The autocomplete request parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Autocomplete suggestions.</returns>
    [HttpGet("autocomplete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NotesAutocompleteResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetAutocomplete(
        [FromQuery] NotesGetAutocompleteRequest request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Section))
        {
            var isSectionNameValid = await sectionsService.IsSectionNameValid(request.Section, cancellationToken);

            if (!isSectionNameValid)
            {
                return ValidationProblem(title: $"Section '{request.Section}' is not valid!");
            }
        }

        var notes = await dbContext.Notes
            .Where(note => string.IsNullOrEmpty(request.Section) || note.Section == request.Section)
            .Where(note => EF.Functions.Like(note.Title, $"%{request.SearchTerm}%"))
            .Select(note => new { note.Title, note.Id })
            .Take(10)
            .ToListAsync(cancellationToken);

        var tags = await tagsService.Autocomplete(request.SearchTerm, request.Section, cancellationToken);
        var books = await booksService.Autocomplete(request.SearchTerm, request.Section, cancellationToken);

        return Ok(new NotesAutocompleteResponse
        {
            Notes = [.. notes.Select(note => new NotesAutocompleteResultItem
            {
                Title = note.Title ?? string.Empty,
                Id = note.Id,
            }).OrderBy(x => x.Title, StringComparer.OrdinalIgnoreCase)],
            Books = books.Select(x => x.Name).ToList(),
            Tags = tags.Select(x => x.Name).ToList(),
        });
    }

    /// <summary>
    /// Gets the number of notes created on each day of a specified month.
    /// </summary>
    /// <param name="request">The calendar days request parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of notes created on each day of the specified month.</returns>
    [HttpGet("calendar")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NotesCalendarDaysResponseItem>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetCalendarDays(
        [FromQuery] NotesGetCalendarDaysRequest request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Section))
        {
            var isSectionNameValid = await sectionsService.IsSectionNameValid(request.Section, cancellationToken);

            if (!isSectionNameValid)
            {
                return ValidationProblem(title: $"Section '{request.Section}' is not valid!");
            }
        }

        var startDate = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Local);
        var endDate = startDate.AddMonths(1).AddSeconds(-1);

        var data = await dbContext.Notes
            .Where(n => n.CreatedAt >= startDate && n.CreatedAt <= endDate)
            .Where(note => string.IsNullOrEmpty(request.Section) || note.Section == request.Section)
            .GroupBy(n => n.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count(),
            })
            .ToListAsync(cancellationToken);

        return Ok(data.Select(x => new NotesCalendarDaysResponseItem { Count = x.Count, Date = x.Date.ToString("yyyy-MM-dd") }));
    }
}
