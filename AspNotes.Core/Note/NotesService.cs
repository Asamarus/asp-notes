using AspNotes.Core.Common;
using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Common.Models;
using AspNotes.Core.Note.Models;
using AspNotes.Core.Tag;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;
using System.Globalization;

namespace AspNotes.Core.Note;

/// <summary>
/// Provides services for managing and querying notes within the application.
/// </summary>
public class NotesService(NotesDbContext context, QueryFactory db) : INotesService
{
    /// <summary>
    /// Checks if a note with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the note to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the note exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> IsNoteIdPresent(long id)
    {
        return await context.Notes.AnyAsync(x => x.Id == id);
    }

    /// <summary>
    /// Searches for notes based on the specified search criteria.
    /// </summary>
    /// <param name="request">The search request containing the criteria.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the search results.</returns>
    public async Task<NotesServiceSearchResult> Search(NotesServiceSearchRequest request)
    {
        var result = new NotesServiceSearchResult();

        var n = new NotesTable("n");

        var query = db.Query()
            .Select(
                n.Id,
                n.CreatedAt,
                n.Title,
                n.Section,
                n.Content,
                n.Preview,
                n.Book,
                n.Tags,
                n.Sources
            )
            .From(n.GetFormattedTableName());

        query.ForPage(request.Page, request.ResultsPerPage);

        if (!string.IsNullOrEmpty(request.Section))
            query.Where(n.Section, request.Section);

        if (!string.IsNullOrEmpty(request.Book))
            query.Where(n.Book, request.Book);

        if (request.Tags.Count > 0)
        {
            query.Where(q =>
            {
                foreach (var tag in request.Tags)
                {
                    q.OrWhereRaw($"{n.Tags} LIKE ?", $"%[{tag}]%");
                }

                return q;
            });
        }

        if (request.WithoutTags)
        {
            query.Where(q =>
            {
                q.Where(n.Tags, "");
                q.OrWhereNull(n.Tags);

                return q;
            });
        }

        if (request.WithoutBook)
        {
            query.Where(q =>
            {
                q.Where(n.Book, "");
                q.OrWhereNull(n.Book);

                return q;
            });
        }

        if (request.FromDate.HasValue)
            query.Where(n.CreatedAt, ">=", request.FromDate.Value.ToString("yyyy-MM-dd") + " 00:00:00");

        if (request.ToDate.HasValue)
            query.Where(n.CreatedAt, "<=", request.ToDate.Value.ToString("yyyy-MM-dd") + " 23:59:59");

        if (!string.IsNullOrEmpty(request.SearchTerm) && request.SearchTerm.Length > 2)
        {
            var ftsResult = await SearchHelper.FullTextSearch(new SearchHelperFullTextSearchRequest
            {
                Query = query,
                FtsSearchColumns = NoteFtsSettings.FtsSearchColumns,
                SearchTerm = request.SearchTerm,
                FtsTableName = NoteFtsSettings.FtsTableName,
                MainContentTableName = NoteFtsSettings.MainContentTableName,
                FtsPrimaryKey = NoteFtsSettings.FtsPrimaryKey,
                MainContentTablePrimaryKey = NoteFtsSettings.MainContentTablePrimaryKey
            });

            query = ftsResult.Query;
            result.SearchTerm = request.SearchTerm;
            result.Keywords = ftsResult.Keywords;
            result.FoundWholePhrase = ftsResult.FoundWholePhrase;
        }

        if (request.InRandomOrder)
            query.OrderByRaw("RANDOM()");
        else
            query.OrderByDesc(n.CreatedAt);

        var notes = await query.GetAsync();

        var notesList = notes.Select(note =>
        {
            var noteDto = new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Section = note.Section,
                Content = note.Content,
                Preview = note.Preview,
                Book = note.Book,
            };

            if (DateTime.TryParseExact(note.CreatedAt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createdAtDateTime))
                noteDto.CreatedAt = createdAtDateTime;

            if (!string.IsNullOrEmpty(note.Tags))
            {
                noteDto.Tags = TagsHelper.Extract(note.Tags);
            }

            if (!string.IsNullOrEmpty(note.Sources))
            {
                var sources = JsonHelper.DeserializeJson<List<NoteSource>>(note.Sources);

                if (sources != null)
                {
                    noteDto.Sources = sources;
                }
            }

            if (!string.IsNullOrEmpty(request.SearchTerm) && request.SearchTerm.Length > 2)
            {
                noteDto.Preview = SearchHelper.GetSearchSnippet(
                    result.Keywords,
                    SearchHelper.GetSearchIndex(note.Content),
                    10,
                    result.FoundWholePhrase
                );
            }

            return noteDto;
        }).ToList();

        query.ClearComponent("offset");
        int totalCount = await query.AsCount().FirstAsync<int>();

        result.Notes = notesList;
        result.Total = totalCount;
        result.Count = notesList.Count;
        result.LastPage = (int)Math.Ceiling((double)totalCount / request.ResultsPerPage);
        result.Page = request.Page;
        result.CanLoadMore = totalCount > request.Page * request.ResultsPerPage;

        return result;
    }

    /// <summary>
    /// Provides autocomplete suggestions for note titles based on the given search term.
    /// </summary>
    /// <param name="searchTerm">The search term for filtering note titles.</param>
    /// <param name="section">Optional. The section to further filter the notes.</param>
    /// <param name="book">Optional. The book to further filter the notes.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of autocomplete suggestions.</returns>
    public async Task<List<NotesServiceAutocompleteResult>> Autocomplete(string searchTerm, string? section = null, string? book = null)
    {
        var n = new NotesTable("n");

        var query = db.Query()
            .Select(n.Id, n.Title)
            .From(n.GetFormattedTableName())
            .Join(NoteFtsSettings.FtsTableName, $"{n.Id}", $"{NoteFtsSettings.FtsTableName}.{NoteFtsSettings.FtsPrimaryKey}")
            .WhereRaw($"{NoteFtsSettings.FtsTableName}.{NoteFtsSettings.FtsTitleColumn} LIKE ?", $"%{searchTerm}%")
            .Limit(10);

        if (!string.IsNullOrEmpty(section))
            query.Where(n.Section, section);

        if (!string.IsNullOrEmpty(book))
            query.Where(n.Book, book);

        var notes = await query.GetAsync();

        return [.. notes.Select(note => new NotesServiceAutocompleteResult
        {
            Id = note.Id,
            Title = note.Title
        }).OrderBy(note => note.Title, StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    /// Gets the number of notes created on each day within a specified month and year, optionally filtered by section.
    /// </summary>
    /// <param name="month">The month for which to retrieve the data.</param>
    /// <param name="year">The year for which to retrieve the data.</param>
    /// <param name="section">Optional. The section to further filter the notes.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of days and the number of notes created on each day.</returns>
    public async Task<List<NotesServiceGetCalendarDaysResult>> GetCalendarDays(int month, int year, string? section = null)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddSeconds(-1);

        var n = new NotesTable("n");

        var query = db.Query()
            .SelectRaw($"COUNT({n.Id}) as Count, date({n.CreatedAt}) as Date")
            .From(n.GetFormattedTableName())
            .WhereRaw($"{n.CreatedAt} >= ?", startDate.ToString("yyyy-MM-dd HH:mm:ss"))
            .WhereRaw($"{n.CreatedAt} <= ?", endDate.ToString("yyyy-MM-dd HH:mm:ss"));

        if (!string.IsNullOrEmpty(section))
        {
            query.Where(n.Section, section);
        }

        query.GroupByRaw("Date");

        var result = await query.GetAsync();

        return [.. result.Select(r => new NotesServiceGetCalendarDaysResult
        {
            Count = r.Count,
            Date = DateOnly.ParseExact((string)r.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
        })];
    }

    /// <summary>
    /// Retrieves a note by its ID.
    /// </summary>
    /// <param name="id">The ID of the note to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the note if found; otherwise, <c>null</c>.</returns>
    public async Task<NoteDto?> GetNoteById(long id)
    {
        var noteEntity = await context.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);

        if (noteEntity == null)
            return null;

        return new NoteDto(noteEntity);
    }

    /// <summary>
    /// Creates a new note within the specified section.
    /// </summary>
    /// <param name="section">The section where the note will be created.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the details of the newly created note.</returns>
    public async Task<NoteDto> CreateNote(string section)
    {
        var noteEntity = new NoteEntity
        {
            Section = section,
        };

        context.Notes.Add(noteEntity);
        await context.SaveChangesAsync();

        return new NoteDto(noteEntity);
    }

    /// <summary>
    /// Updates the section of a note identified by its ID.
    /// </summary>
    /// <param name="id">The ID of the note to update.</param>
    /// <param name="section">The new section to assign to the note.</param>
    /// <returns>The updated NoteDto object if the update is successful; otherwise, null.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a note with the specified ID is not found.</exception>
    public async Task<NoteDto> UpdateNoteSection(long id, string section)
    {
        var noteEntity = await context.Notes.FindAsync(id) ?? throw new InvalidOperationException("Note not found.");

        noteEntity.Section = section;

        await context.SaveChangesAsync();

        return new NoteDto(noteEntity);
    }

    /// <summary>
    /// Updates the title and content of a note identified by its ID.
    /// </summary>
    /// <param name="id">The ID of the note to update.</param>
    /// <param name="title">The new title for the note.</param>
    /// <param name="content">The new content of the note.</param>
    /// <returns>The updated NoteDto object if the update is successful; otherwise, null.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a note with the specified ID is not found.</exception>
    public async Task<NoteDto> UpdateNoteTitleAndContent(long id, string title, string content)
    {
        var noteEntity = await context.Notes.FindAsync(id) ?? throw new InvalidOperationException("Note not found.");

        if (noteEntity.Title != title)
        {
            noteEntity.TitleSearchIndex = string.IsNullOrEmpty(title) ? null : SearchHelper.GetSearchIndex(title);
            noteEntity.Title = title;
        }

        if (noteEntity.Content != content)
        {
            noteEntity.ContentSearchIndex = string.IsNullOrEmpty(content) ? null : SearchHelper.GetSearchIndex(content);
            noteEntity.Preview = string.IsNullOrEmpty(content) ? null : SearchHelper.GetSearchIndex(content, false, 100);
            noteEntity.Content = content;
        }

        await context.SaveChangesAsync();

        return new NoteDto(noteEntity);
    }

    /// <summary>
    /// Updates the sources of a note identified by its ID.
    /// </summary>
    /// <param name="id">The ID of the note to update.</param>
    /// <param name="sources">The new list of sources to assign to the note.</param>
    /// <returns>The updated NoteDto object if the update is successful; otherwise, null.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a note with the specified ID is not found.</exception>
    public async Task<NoteDto> UpdateNoteSources(long id, List<NoteSource> sources)
    {
        var noteEntity = await context.Notes.FindAsync(id) ?? throw new InvalidOperationException("Note not found.");

        noteEntity.Sources = sources;

        await context.SaveChangesAsync();

        return new NoteDto(noteEntity);
    }

    /// <summary>
    /// Deletes a note by its ID.
    /// </summary>
    /// <param name="id">The ID of the note to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the note was successfully deleted; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a note with the specified ID is not found.</exception>
    public async Task<bool> DeleteNote(long id)
    {
        var noteEntity = await context.Notes.FindAsync(id) ?? throw new InvalidOperationException("Note not found.");

        context.Notes.Remove(noteEntity);
        await context.SaveChangesAsync();

        return true;
    }
}
