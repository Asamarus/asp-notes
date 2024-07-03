﻿using AspNotes.Core.Common;
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

        var notes = await query.GetAsync();

        var dateFormats = new string[] { "yyyy-MM-dd HH:mm:ss.fffffff", "yyyy-MM-dd HH:mm:ss" };

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

            if (DateTime.TryParseExact(note.CreatedAt, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createdAtDateTime))
                noteDto.CreatedAt = createdAtDateTime;

            if (!string.IsNullOrEmpty(note.Tags))
            {
                noteDto.Tags = TagsHelper.Extract(note.Tags);
            }

            if (!string.IsNullOrEmpty(note.Sources))
            {
                var sources = JsonHelper.DeserializeJson<NoteSource>(note.Sources);

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

        result.Rows = notesList;
        result.Total = totalCount;
        result.Count = notesList.Count;
        result.LastPage = (int)Math.Ceiling((double)totalCount / request.ResultsPerPage);
        result.Page = request.Page;
        result.LoadMore = totalCount > request.Page * request.ResultsPerPage;

        return result;
    }

    /// <summary>
    /// Provides autocomplete suggestions for note titles based on the given search term.
    /// </summary>
    /// <param name="searchTerm">The search term for filtering note titles.</param>
    /// <param name="section">Optional. The section to further filter the notes.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of autocomplete suggestions.</returns>
    public async Task<List<NotesServiceAutocompleteResult>> Autocomplete(string searchTerm, string? section = null)
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

        var notes = await query.GetAsync();

        return [.. notes.Select(note => new NotesServiceAutocompleteResult
        {
            Id = note.Id.ToString(),
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
            .SelectRaw($"COUNT({n.Id}) as Number, date({n.CreatedAt}) as Date")
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
            Number = r.Number,
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
        var noteEntity = await context.Notes.FindAsync(id);

        if (noteEntity == null)
            return null;

        return new NoteDto(noteEntity);
    }

    /// <summary>
    /// Creates a new note with the specified details.
    /// </summary>
    /// <param name="note">The details of the note to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the details of the created note.</returns>
    public async Task<NoteDto> CreateNote(NoteDto note)
    {
        var noteEntity = new NoteEntity
        {
            Title = note.Title,
            Section = note.Section,
            Content = note.Content,
            Book = note.Book,
        };

        context.Notes.Add(noteEntity);
        await context.SaveChangesAsync();

        return new NoteDto(noteEntity);
    }

    /// <summary>
    /// Updates an existing note with the specified details.
    /// </summary>
    /// <param name="note">The new details of the note to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated note details.</returns>
    public async Task<NoteDto> UpdateNote(NoteDto note)
    {
        var noteEntity = await context.Notes.FindAsync(note.Id) ?? throw new InvalidOperationException("Note not found.");
        noteEntity.Title = note.Title;
        noteEntity.Content = note.Content;
        noteEntity.Sources = note.Sources;

        await context.SaveChangesAsync();

        return new NoteDto(noteEntity);
    }

    /// <summary>
    /// Deletes a note by its ID.
    /// </summary>
    /// <param name="id">The ID of the note to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the note was successfully deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteNote(long id)
    {
        var noteEntity = await context.Notes.FindAsync(id) ?? throw new InvalidOperationException("Note not found.");

        context.Notes.Remove(noteEntity);
        await context.SaveChangesAsync();

        return true;
    }
}
