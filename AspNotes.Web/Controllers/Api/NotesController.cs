using AspNotes.Core.Book;
using AspNotes.Core.Note;
using AspNotes.Core.Note.Models;
using AspNotes.Core.Section;
using AspNotes.Core.Tag;
using AspNotes.Web.Models.Common;
using AspNotes.Web.Models.Notes;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Web.Controllers.Api;

[Route("api/notes")]
[ApiController]
[Produces("application/json")]
public class NotesController(
    INotesService notesService,
    ISectionsService sectionsService,
    IBooksService booksService,
    ITagsService tagsService
    ) : ControllerBase
{
    /// <summary>
    /// Searches for notes based on the provided search criteria.
    /// </summary>
    /// <param name="request">The search criteria for notes.</param>
    /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the asynchronous operation, including the search results or an error response.</returns>
    /// <response code="200">Returns the search results.</response>
    /// <response code="400">If the search criteria are not valid, for example, if a specified section does not exist.</response>
    [HttpPost("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchNotesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> SearchNotes(SearchNotesRequest request)
    {
        if (!string.IsNullOrEmpty(request.Section) && !await sectionsService.IsSectionNameValid(request.Section))
            return BadRequest(new ErrorResponse { Message = $"Section with Name '{request.Section}' is not found!" });

        var searchRequest = new NotesServiceSearchRequest
        {
            Section = request.Section,
            SearchTerm = request.SearchTerm,
            Page = request.Page,
            Book = request.Book,
            Tags = new List<string>(request.Tags),
            InRandomOrder = request.InRandomOrder,
            WithoutBook = request.WithoutBook,
            WithoutTags = request.WithoutTags,
            FromDate = request.FromDate,
            ToDate = request.ToDate
        };

        var searchResponse = await notesService.Search(searchRequest);

        var response = new SearchNotesResponse
        {
            Notes = searchResponse.Notes.Select(note => new NoteItemResponse(note)).ToList(),
            Total = searchResponse.Total,
            Count = searchResponse.Count,
            LastPage = searchResponse.LastPage,
            CanLoadMore = searchResponse.CanLoadMore,
            Page = searchResponse.Page,
            SearchTerm = searchResponse.SearchTerm,
            Keywords = new HashSet<string>(searchResponse.Keywords),
            FoundWholePhrase = searchResponse.FoundWholePhrase
        };

        return Ok(response);
    }

    /// <summary>
    /// Provides autocomplete suggestions for notes, books, and tags based on the provided search term and optional section.
    /// </summary>
    /// <param name="request">The autocomplete request containing the search term and optional section.</param>
    /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the asynchronous operation, including the autocomplete suggestions or an error response.</returns>
    /// <response code="200">Returns the autocomplete suggestions.</response>
    /// <response code="400">If the search criteria are not valid, for example, if a specified section does not exist.</response>
    [HttpPost("autocomplete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutocompleteNotesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> AutocompleteNotes(AutocompleteNotesRequest request)
    {
        if (!string.IsNullOrEmpty(request.Section) && !await sectionsService.IsSectionNameValid(request.Section))
            return BadRequest(new ErrorResponse { Message = $"Section with Name '{request.Section}' is not found!" });

        var notes = await notesService.Autocomplete(request.SearchTerm, request.Section);
        var books = await booksService.Autocomplete(request.SearchTerm, request.Section);
        var tags = await tagsService.Autocomplete(request.SearchTerm, request.Section);

        var response = new AutocompleteNotesResponse
        {
            Notes = notes.Select(x => new AutoCompleteNotesItemResponse { Id = x.Id, Title = x.Title }).ToList(),
            Books = books.Select(x => x.Name).ToHashSet(),
            Tags = tags.Select(x => x.Name).ToHashSet(),
        };

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a specific note by its ID.
    /// </summary>
    /// <param name="request">The request containing the ID of the note to retrieve.</param>
    /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the asynchronous operation, including the note details or an error response.</returns>
    /// <response code="200">Returns the requested note details.</response>
    /// <response code="400">If the note with the specified ID is not found.</response>
    [HttpPost("get")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetNote(GetNoteRequest request)
    {
        var note = await notesService.GetNoteById(request.Id);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.Id}' is not found!" });

        var response = new NoteResponse
        {
            Note = new NoteItemResponse(note),
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a new note in the specified section.
    /// </summary>
    /// <param name="request">The request containing the details for creating a new note, including the section and optional book.</param>
    /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the asynchronous operation, including the created note details or an error response.</returns>
    /// <response code="200">Returns the details of the created note.</response>
    /// <response code="400">If the specified section is not found or the book update fails.</response>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateNote(CreateNoteRequest request)
    {
        if (!await sectionsService.IsSectionNameValid(request.Section))
            return BadRequest(new ErrorResponse { Message = $"Section with Name '{request.Section}' is not found!" });

        var note = await notesService.CreateNote(request.Section);

        if (!string.IsNullOrEmpty(request.Book))
        {
            if (!await booksService.UpdateNoteBook(note.Id, request.Book))
                return BadRequest(new ErrorResponse { Message = $"Book update failed!" });

            note = await notesService.GetNoteById(note.Id);

            if (note == null)
                return BadRequest(new ErrorResponse { Message = $"Note is not found!" });
        }

        var response = new NoteResponse
        {
            Message = "Note is created!",
            Note = new NoteItemResponse(note),
        };

        return Ok(response);
    }

    /// <summary>
    /// Updates the title and content of an existing note.
    /// </summary>
    /// <param name="request">The request containing the ID of the note to update along with the new title and content.</param>
    /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the asynchronous operation, including the updated note details or an error response.</returns>
    /// <response code="200">Returns the updated note details.</response>
    /// <response code="400">If the note with the specified ID is not found.</response>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateNote(UpdateNoteRequest request)
    {
        var note = await notesService.GetNoteById(request.Id);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.Id}' is not found!" });

        note = await notesService.UpdateNoteTitleAndContent(note.Id, request.Title, request.Content);

        var response = new NoteResponse
        {
            Message = "Note is saved!",
            ShowNotification = true,
            Note = new NoteItemResponse(note)
        };

        return Ok(response);
    }

    /// <summary>
    /// Updates the book associated with a specific note.
    /// </summary>
    /// <param name="request">The request containing the ID of the note and the new book details.</param>
    /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the asynchronous operation, including a success message or an error response.</returns>
    /// <response code="200">Returns a success message and the updated note details.</response>
    /// <response code="400">If the note with the specified ID is not found or the book update fails.</response>
    [HttpPost("updateBook")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateNoteBook(UpdateNoteBookRequest request)
    {
        var note = await notesService.GetNoteById(request.Id);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.Id}' is not found!" });

        if (!await booksService.UpdateNoteBook(note.Id, request.Book))
            return BadRequest(new ErrorResponse { Message = $"Book update failed!" });

        note = await notesService.GetNoteById(request.Id);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.Id}' is not found!" });

        var response = new NoteResponse
        {
            Message = "Note book is updated!",
            ShowNotification = true,
            Note = new NoteItemResponse(note)
        };

        return Ok(response);
    }

    /// <summary>
    /// Updates the tags associated with a specific note.
    /// </summary>
    /// <param name="request">The request containing the ID of the note and the new tags.</param>
    /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the asynchronous operation, including a success message or an error response.</returns>
    /// <response code="200">Returns a success message and the updated note details.</response>
    /// <response code="400">If the note with the specified ID is not found or the tags update fails.</response>
    [HttpPost("updateTags")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateNoteTags(UpdateNoteTagsRequest request)
    {
        var note = await notesService.GetNoteById(request.Id);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.Id}' is not found!" });

        if (!await tagsService.UpdateNoteTags(note.Id, request.Tags))
            return BadRequest(new ErrorResponse { Message = $"Tags update failed!" });

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.Id}' is not found!" });

        var response = new NoteResponse
        {
            Message = "Note tags are updated!",
            ShowNotification = true,
            Note = new NoteItemResponse(note)
        };

        return Ok(response);
    }

    /// <summary>
    /// Updates the section of a specific note.
    /// </summary>
    /// <param name="request">The request containing the ID of the note and the new section name.</param>
    /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the asynchronous operation, including a success message or an error response.</returns>
    /// <response code="200">Returns a success message indicating the note has been moved to another section, along with the updated note details.</response>
    /// <response code="400">If the section name is invalid, the note is not found, or updates to the book or tags fail.</response>
    [HttpPost("updateSection")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateNoteSection(UpdateNoteSectionRequest request)
    {
        if (!await sectionsService.IsSectionNameValid(request.Section))
            return BadRequest(new ErrorResponse { Message = $"Section with Name '{request.Section}' is not found!" });

        var note = await notesService.GetNoteById(request.Id);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.Id}' is not found!" });

        if (note.Section == request.Section)
            return BadRequest(new ErrorResponse { Message = $"Note is already in the '{request.Section}' section!" });

        var book = note.Book ?? string.Empty;
        var tags = new List<string>(note.Tags).ToHashSet();

        // Update book and tags to empty values to clear book and tags for current section
        if (!await booksService.UpdateNoteBook(note.Id, string.Empty))
            return BadRequest(new ErrorResponse { Message = $"Book update failed!" });

        if (!await tagsService.UpdateNoteTags(note.Id, []))
            return BadRequest(new ErrorResponse { Message = $"Tags update failed!" });

        note = await notesService.UpdateNoteSection(note.Id, request.Section);

        // Update book and tags back to original values
        if (!await booksService.UpdateNoteBook(note.Id, book))
            return BadRequest(new ErrorResponse { Message = $"Book update failed!" });

        if (!await tagsService.UpdateNoteTags(note.Id, tags))
            return BadRequest(new ErrorResponse { Message = $"Tags update failed!" });

        note = await notesService.GetNoteById(request.Id);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.Id}' is not found!" });

        var response = new NoteResponse
        {
            Message = "Note is moved to another section!",
            ShowNotification = true,
            Note = new NoteItemResponse(note)
        };

        return Ok(response);
    }

    /// <summary>
    /// Deletes a specific note by its ID.
    /// </summary>
    /// <param name="request">The request containing the ID of the note to be deleted.</param>
    /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the asynchronous operation, including a success message or an error response.</returns>
    /// <response code="200">Returns a success message indicating the note has been deleted.</response>
    /// <response code="400">If the note is not found or the deletion fails.</response>
    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> DeleteNote(DeleteNoteRequest request)
    {
        var note = await notesService.GetNoteById(request.Id);

        if (note == null)
            return BadRequest(new ErrorResponse { Message = $"Note with id '{request.Id}' is not found!" });

        if (!await notesService.DeleteNote(request.Id))
            return BadRequest(new ErrorResponse { Message = $"Note delete failed!" });

        var response = new SuccessResponse
        {
            Message = "Note is deleted!",
            ShowNotification = true,
        };

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a list of days within a specified month and year, optionally filtered by section, that contain notes.
    /// </summary>
    /// <param name="request">The request containing the month, year, and optional section to filter the calendar days.</param>
    /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the asynchronous operation, including a list of days with notes or an error response.</returns>
    /// <response code="200">Returns a list of calendar days that contain notes, each with the date and the number of notes for that day.</response>
    [HttpPost("getCalendarDays")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NoteCalendarDaysResponseItem>))]
    public async Task<IActionResult> GetCalendarDays(GetNoteCalendarDaysRequest request)
    {
        var days = await notesService.GetCalendarDays(request.Month, request.Year, request.Section);

        var response = days
            .Select(x => new NoteCalendarDaysResponseItem { Date = x.Date, Count = x.Count })
            .ToList();

        return Ok(response);
    }
}