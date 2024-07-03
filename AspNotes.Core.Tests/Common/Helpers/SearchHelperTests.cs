using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Common.Models;
using AspNotes.Core.Note;
using AspNotes.Core.Note.Models;
using SqlKata.Execution;

namespace AspNotes.Core.Tests.Common.Helpers;

public class SearchHelperTests : DatabaseTestBase
{
    private readonly SearchHelperFullTextSearchRequest _fullTextSearchRequest;
    private readonly NotesService _notesService;

    public SearchHelperTests()
    {
        _fullTextSearchRequest = new SearchHelperFullTextSearchRequest
        {
            Query = DbFixture.Db.Query(),
            FtsSearchColumns = NoteFtsSettings.FtsSearchColumns,
            SearchTerm = string.Empty,
            FtsTableName = NoteFtsSettings.FtsTableName,
            MainContentTableName = NoteFtsSettings.MainContentTableName,
            FtsPrimaryKey = NoteFtsSettings.FtsPrimaryKey,
            MainContentTablePrimaryKey = NoteFtsSettings.MainContentTablePrimaryKey
        };

        _notesService = new NotesService(DbFixture.DbContext, DbFixture.Db);
    }

    [Fact]
    public void GetSearchIndex_ShouldProcessStringCorrectly()
    {
        // Arrange
        var content = "<p>Hello</p>   <p>world</p>";
        var expected = "hello world";

        // Act
        var result = SearchHelper.GetSearchIndex(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetSearchIndex_ShouldConvertToLowercase()
    {
        // Arrange
        var content = "<p>HELLO</p>   <p>WORLD</p>";
        var expected = "hello world";

        // Act
        var result = SearchHelper.GetSearchIndex(content, true);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetSearchIndex_ShouldLimitLength()
    {
        // Arrange
        var content = "<p>Hello</p>   <p>World</p>";
        var expected = "hello...";

        // Act
        var result = SearchHelper.GetSearchIndex(content, true, 6);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetSearchIndex_ShouldSeparateWordsInDifferentHtmlTagsWithSpace()
    {
        // Arrange
        var content = "<p>Hello</p><p>world</p>";
        var expected = "Hello world";

        // Act
        var result = SearchHelper.GetSearchIndex(content, false);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldReturnEmptyString_WhenKeywordsAreEmpty()
    {
        // Arrange
        var keywords = new HashSet<string>();
        var text = "Hello, World!";

        // Act
        var result = SearchHelper.GetSearchSnippet(keywords, text);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldReturnEmptyString_WhenTextIsEmpty()
    {
        // Arrange
        var keywords = new HashSet<string> { "hello" };
        var text = string.Empty;

        // Act
        var result = SearchHelper.GetSearchSnippet(keywords, text);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldReturnSnippet_WhenKeywordIsFound()
    {
        // Arrange
        var keywords = new HashSet<string> { "hello" };
        var text = "Hello, World!";

        // Act
        var result = SearchHelper.GetSearchSnippet(keywords, text);

        // Assert
        Assert.Equal("Hello, World!...", result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldHighlightKeyword_WhenHighlightIsTrue()
    {
        // Arrange
        var keywords = new HashSet<string> { "hello" };
        var text = "Hello, World!";
        var highlight = true;

        // Act
        var result = SearchHelper.GetSearchSnippet(keywords, text, highlight: highlight);

        // Assert
        Assert.Equal("<em>Hello</em>, World!...", result);
    }

    [Fact]
    public void GetSearchSnippet_ShouldReturnCorrectSnippet_WithHighlightedKeywords()
    {
        // Arrange
        var keywords = new HashSet<string> { "lorem", "ipsum", "dolor" };
        var text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed non risus. Suspendisse lectus tortor, dignissim sit amet, adipiscing nec, ultricies sed, dolor. Cras elementum ultrices diam. Maecenas ligula massa, varius a, semper congue, euismod non, mi.";
        var highlight = true;

        // Act
        var result = SearchHelper.GetSearchSnippet(keywords, text, highlight: highlight);

        // Assert
        Assert.Contains("<em>Lorem</em> ipsum dolor si...", result);
        Assert.Contains("Lorem <em>ipsum</em> dolor sit amet...", result);
        Assert.Contains("Lorem ipsum <em>dolor</em> sit amet, cons...", result);
        Assert.Contains("ultricies sed, <em>dolor</em>. Cras elementu...", result);
    }

    [Fact]
    public void GetSearchSnippet_WithWholePhraseHighlighting_ReturnsExpectedSnippet()
    {
        // Arrange
        var keywords = new HashSet<string> { "hello world" };
        var text = "This is a test for Hello World highlighting.";
        var expectedSnippet = "is a test for <em>Hello World</em> highlighting....";

        // Act
        var snippet = SearchHelper.GetSearchSnippet(keywords, text, foundWholePhrase: true, highlight: true);

        // Assert
        Assert.Equal(expectedSnippet, snippet);
    }

    [Fact]
    public async Task FullTextSearch_FindsWholePhrase_ReturnsExpectedResult()
    {
        // Arrange
        var searchPhrase = "This is some string";
        await _notesService.CreateNote(new NoteDto
        {
            Title = "Note 1",
            Content = searchPhrase,
            Section = "section1"
        });

        var n = new NotesTable("n");

        var notesQuery = DbFixture.Db.Query()
            .Select(n.Id, n.Title, n.Content)
            .From(n.GetFormattedTableName())
            .Where(n.Section, "section1");

        _fullTextSearchRequest.Query = notesQuery;
        _fullTextSearchRequest.SearchTerm = searchPhrase;

        // Act
        var result = await SearchHelper.FullTextSearch(_fullTextSearchRequest);
        var notes = await result.Query.GetAsync<NoteDto>();

        // Assert
        Assert.True(result.FoundWholePhrase);
        Assert.Contains(searchPhrase, result.Keywords);
        Assert.Single(notes);
        Assert.Equal(searchPhrase, notes.First().Content);
    }

    [Fact]
    public async Task FullTextSearch_FindsWithFts_ReturnsExpectedResult()
    {
        // Arrange
        var searchPhrase = "text some";
        await _notesService.CreateNote(new NoteDto
        {
            Title = "Note 1",
            Content = "Some text to find",
            Section = "section1"
        });

        var n = new NotesTable("n");

        var notesQuery = DbFixture.Db.Query()
            .Select(n.Id, n.Title, n.Content)
            .From(n.GetFormattedTableName())
            .Where(n.Section, "section1");

        _fullTextSearchRequest.Query = notesQuery;
        _fullTextSearchRequest.SearchTerm = searchPhrase;

        // Act
        var result = await SearchHelper.FullTextSearch(_fullTextSearchRequest);
        var notes = await result.Query.GetAsync<NoteDto>();

        // Assert
        Assert.False(result.FoundWholePhrase);
        Assert.Contains("text", result.Keywords);
        Assert.Contains("some", result.Keywords);
        Assert.Single(notes);
    }

    [Fact]
    public async Task FullTextSearch_FindsWithKeywords_ReturnsExpectedResult()
    {
        // Arrange
        var searchPhrase = "keyword unique";
        await _notesService.CreateNote(new NoteDto
        {
            Title = "Note 2",
            Content = "This contains the unique keyword",
            Section = "section2"
        });

        var n = new NotesTable("n");

        var notesQuery = DbFixture.Db.Query()
            .Select(n.Id, n.Title, n.Content)
            .From(n.GetFormattedTableName())
            .Where(n.Section, "section2");

        _fullTextSearchRequest.Query = notesQuery;
        _fullTextSearchRequest.SearchTerm = searchPhrase;

        // Act
        var result = await SearchHelper.FullTextSearch(_fullTextSearchRequest);
        var notes = await result.Query.GetAsync<NoteDto>();

        // Assert
        Assert.Contains("unique", result.Keywords);
        Assert.Contains("keyword", result.Keywords);
        Assert.NotEmpty(notes);
    }

    [Fact]
    public async Task FullTextSearch_FindsKeywordsInsideString_ReturnsExpectedResult()
    {
        // Arrange
        var searchPhrase = "partial xyz";
        await _notesService.CreateNote(new NoteDto
        {
            Title = "Note 3",
            Content = "This note contains a somepartialmatch",
            Section = "section3"
        });

        var n = new NotesTable("n");

        var notesQuery = DbFixture.Db.Query()
            .Select(n.Id, n.Title, n.Content)
            .From(n.GetFormattedTableName())
            .Where(n.Section, "section3");

        _fullTextSearchRequest.Query = notesQuery;
        _fullTextSearchRequest.SearchTerm = searchPhrase;

        // Act
        var result = await SearchHelper.FullTextSearch(_fullTextSearchRequest);
        var notes = await result.Query.GetAsync<NoteDto>();

        // Assert
        Assert.False(result.FoundWholePhrase);
        Assert.Contains("partial", result.Keywords);
        Assert.Contains("xyz", result.Keywords);
        Assert.NotEmpty(notes);
    }

    [Fact]
    public async Task FullTextSearch_ReduceKeywordsUntilResultsFound_ReturnsExpectedResult()
    {
        // Arrange
        var searchPhrase = "longkeyword";
        await _notesService.CreateNote(new NoteDto
        {
            Title = "Note 4",
            Content = "This contains longkey",
            Section = "section4"
        });

        var n = new NotesTable("n");

        var notesQuery = DbFixture.Db.Query()
            .Select(n.Id, n.Title, n.Content)
            .From(n.GetFormattedTableName())
            .Where(n.Section, "section4");

        _fullTextSearchRequest.Query = notesQuery;
        _fullTextSearchRequest.SearchTerm = searchPhrase;

        // Act
        var result = await SearchHelper.FullTextSearch(_fullTextSearchRequest);
        var notes = await result.Query.GetAsync<NoteDto>();

        // Assert
        Assert.False(result.FoundWholePhrase);
        Assert.Contains("longkey", result.Keywords);
        Assert.NotEmpty(notes);
    }

    [Fact]
    public async Task FullTextSearch_NoResultsFound_ReturnsExpectedResult()
    {
        // Arrange
        var searchPhrase = "not found";
        await _notesService.CreateNote(new NoteDto
        {
            Title = "Title",
            Content = "This is text",
            Section = "section1"
        });

        var n = new NotesTable("n");

        var notesQuery = DbFixture.Db.Query()
            .Select(n.Id, n.Title, n.Content)
            .From(n.GetFormattedTableName())
            .Where(n.Section, "section1");

        _fullTextSearchRequest.Query = notesQuery;
        _fullTextSearchRequest.SearchTerm = searchPhrase;

        // Act
        var result = await SearchHelper.FullTextSearch(_fullTextSearchRequest);
        var notes = await result.Query.GetAsync<NoteDto>();

        // Assert
        Assert.False(result.FoundWholePhrase);
        Assert.Contains("not", result.Keywords);
        Assert.Contains("fou", result.Keywords);
        Assert.Empty(notes);
    }
}
