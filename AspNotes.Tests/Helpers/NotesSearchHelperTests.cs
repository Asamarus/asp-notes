using AspNotes.Entities;
using AspNotes.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AspNotes.Tests.Helpers;

public class NotesSearchHelperTests : InMemoryDatabaseTestBase
{

    [Fact]
    public async Task FullTextSearch_ShouldFindWholePhrase()
    {
        // Arrange
        var searchPhrase = "This is some string";
        var note = new NoteEntity
        {
            Content = searchPhrase,
            ContentSearchIndex = SearchHelper.GetSearchIndex(searchPhrase),
            Section = "Test",
        };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        var mainQuery = DbContext.Notes.AsQueryable();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await NotesSearchHelper.FullTextSearch(mainQuery, searchPhrase, cancellationToken);
        var notes = await result.Query.ToListAsync();

        // Assert
        Assert.True(result.FoundWholePhrase);
        Assert.Contains(searchPhrase, result.Keywords);
        Assert.Single(notes);
        Assert.Equal(searchPhrase, notes[0].Content);
    }


    [Fact]
    public async Task FullTextSearch_ShouldFindKeywords()
    {
        // Arrange
        var searchPhrase = "text some";
        var note = new NoteEntity
        {
            ContentSearchIndex = SearchHelper.GetSearchIndex("Some text to find"),
            Section = "Test",
        };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        var mainQuery = DbContext.Notes.AsQueryable();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await NotesSearchHelper.FullTextSearch(mainQuery, searchPhrase, cancellationToken);
        var notes = await result.Query.ToListAsync();

        // Assert
        Assert.False(result.FoundWholePhrase);
        Assert.Contains("text", result.Keywords);
        Assert.Contains("some", result.Keywords);
        Assert.Single(notes);
    }

    [Fact]
    public async Task FullTextSearch_ShouldFindPartialKeyword()
    {
        // Arrange
        var searchPhrase = "partial xyz";
        var note = new NoteEntity
        {
            ContentSearchIndex = SearchHelper.GetSearchIndex("This note contains a somepartialmatch"),
            Section = "Test",
        };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        var mainQuery = DbContext.Notes.AsQueryable();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await NotesSearchHelper.FullTextSearch(mainQuery, searchPhrase, cancellationToken);
        var notes = await result.Query.ToListAsync();

        // Assert
        Assert.False(result.FoundWholePhrase);
        Assert.Contains("partial", result.Keywords);
        Assert.Contains("xyz", result.Keywords);
        Assert.Single(notes);
    }

    [Fact]
    public async Task FullTextSearch_ShouldReduceKeywordsUntilResultIsFound()
    {
        // Arrange
        var searchPhrase = "longkeyword";
        var note = new NoteEntity
        {
            ContentSearchIndex = SearchHelper.GetSearchIndex("This contains longkey"),
            Section = "Test",
        };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        var mainQuery = DbContext.Notes.AsQueryable();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await NotesSearchHelper.FullTextSearch(mainQuery, searchPhrase, cancellationToken);
        var notes = await result.Query.ToListAsync();

        // Assert
        Assert.False(result.FoundWholePhrase);
        Assert.Contains("longkey", result.Keywords);
        Assert.Single(notes);
    }

    [Fact]
    public async Task FullTextSearch_ShouldNotFindNonExistentPhrase()
    {
        // Arrange
        var searchPhrase = "not found";
        var note = new NoteEntity
        {
            ContentSearchIndex = SearchHelper.GetSearchIndex("This is text"),
            Section = "Test",
        };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        var mainQuery = DbContext.Notes.AsQueryable();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await NotesSearchHelper.FullTextSearch(mainQuery, searchPhrase, cancellationToken);
        var notes = await result.Query.ToListAsync();

        // Assert
        Assert.False(result.FoundWholePhrase);
        Assert.Contains("not", result.Keywords);
        Assert.Contains("fou", result.Keywords);
        Assert.Empty(notes);
    }
}
