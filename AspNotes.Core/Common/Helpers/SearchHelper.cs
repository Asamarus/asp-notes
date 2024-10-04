using AspNotes.Core.Common.Models;
using SqlKata;
using SqlKata.Execution;
using System.Text.RegularExpressions;
using System.Web;

namespace AspNotes.Core.Common.Helpers;

/// <summary>
/// Provides helper methods for search functionality.
/// </summary>
public static partial class SearchHelper
{
    /// <summary>
    /// Generates a search index from the provided content.
    /// </summary>
    /// <param name="content">The content to index.</param>
    /// <param name="toLower">If set to true, the content will be converted to lower case.</param>
    /// <param name="limit">The maximum length of the returned string. If 0, the entire content is returned.</param>
    /// <returns>A string that represents the search index.</returns>
    public static string GetSearchIndex(string content, bool toLower = true, int limit = 0)
    {
        var searchIndex = content.Trim();

        //trick to save spaces for words in separate html tags
        //example: <p>Hello</p><p>world</p> will be 'Hello world' and not 'Helloworld' in search index
        searchIndex = searchIndex.Replace("<", " <");
        searchIndex = HtmlHelper.StripTags(searchIndex);
        searchIndex = MultipleSpaceRegex().Replace(searchIndex, " ");

        searchIndex = HttpUtility.HtmlDecode(searchIndex);

        if (toLower)
            searchIndex = searchIndex.ToLower();

        if (limit > 0 && limit < searchIndex.Length)
            searchIndex = searchIndex[..limit].Trim() + "...";

        return searchIndex.Trim();
    }

    /// <summary>
    /// Generates a Regex object that matches multiple spaces.
    /// </summary>
    /// <returns>A Regex object that matches multiple spaces.</returns>
    [GeneratedRegex("  +")]
    private static partial Regex MultipleSpaceRegex();

    /// <summary>
    /// Generates a search snippet from the provided text based on a list of keywords.
    /// </summary>
    /// <param name="keywords">A list of keywords to search for in the text.</param>
    /// <param name="text">The text to search in.</param>
    /// <param name="limit">The maximum number of snippets to return. If 0, all snippets are returned.</param>
    /// <param name="foundWholePhrase">A flag indicating whether the whole phrase was found in the text.</param>
    /// <param name="highlight">A flag indicating whether to highlight the keywords in the returned snippets.</param>
    /// <returns>A string that represents the search snippet. If no snippets are found, an empty string is returned.</returns>
    public static string GetSearchSnippet(
        HashSet<string> keywords,
        string text,
        int limit = 10,
        bool foundWholePhrase = false,
        bool highlight = false
    )
    {
        int snippetCurrent = 0;
        const int radius = 15;

        if (keywords.Count == 0 || string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        if (!foundWholePhrase)
        {
            var newKeywords = new HashSet<string>();
            foreach (var keyword in keywords)
            {
                var words = keyword.Split(' ');
                foreach (var word in words)
                {
                    if (word.Length > 2)
                        newKeywords.Add(word);
                }
            }
            keywords = newKeywords;
        }

        var result = new List<string>();
        var regexDict = new Dictionary<string, (Regex matchRegex, Regex? replaceRegex)>();

        foreach (var keyword in keywords)
        {
            if (!regexDict.TryGetValue(keyword, out var value))
            {
                string matchPattern = $"(.{{0,{radius}}})({Regex.Escape(keyword)})(.{{0,{radius}}})";
                try
                {
                    var matchRegex = new Regex(matchPattern, RegexOptions.IgnoreCase);
                    Regex? replaceRegex = null;
                    if (highlight)
                    {
                        string replacePattern = $"({Regex.Escape(keyword)})";
                        replaceRegex = new Regex(replacePattern, RegexOptions.IgnoreCase);
                    }
                    value = (matchRegex, replaceRegex);
                }
                catch (Exception)
                {
                    continue;
                }
                regexDict[keyword] = value;
            }

            var regex = value;

            var matches = value.matchRegex.Matches(text);

            if (matches.Count != 0)
            {
                foreach (Match match in matches)
                {
                    var trimmedMatch = match.Value.Trim();

                    if (string.IsNullOrEmpty(trimmedMatch))
                        continue;

                    if (snippetCurrent >= limit)
                        return string.Join(" ", result);

                    if (highlight && value.replaceRegex != null)
                        trimmedMatch = value.replaceRegex.Replace(trimmedMatch, "<em>$1</em>");

                    result.Add($"{trimmedMatch}...");

                    snippetCurrent++;
                }
            }
        }

        return string.Join(" ", result);
    }

    /// <summary>
    /// Performs a full text search based on the provided request parameters.
    /// </summary>
    /// <param name="request">The parameters for the full text search, including the search term, the columns to search in.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="SearchHelperFullTextSearchResult"/> object with the search results.</returns>
    public static async Task<SearchHelperFullTextSearchResult> FullTextSearch(SearchHelperFullTextSearchRequest request)
    {
        var mainQuery = request.Query.Clone();

        var result = new SearchHelperFullTextSearchResult
        {
            Query = mainQuery,
            Keywords = [request.SearchTerm]
        };

        // check if whole phrase is found
        var query = mainQuery.Clone();
        query.AddLikeSearch(request, [request.SearchTerm]);

        if (await HasResults(query))
        {
            result.Query = query;
            result.FoundWholePhrase = true;
            return result;
        }

        var keywords = request.SearchTerm.Split(' ')
               .Where(word => word.Length >= 3)
               .ToHashSet();

        result.Keywords = keywords;

        // try FTS
        query = mainQuery.Clone();
        query.AddFullTextSearch(request, [request.SearchTerm]);

        if (await HasResults(query))
        {
            result.Query = query;
            return result;
        }

        query = mainQuery.Clone();
        query.AddFullTextSearch(request, keywords);

        if (await HasResults(query))
        {
            result.Query = query;
            return result;
        }

        query = mainQuery.Clone();
        query.AddLikeSearch(request, keywords);

        if (await HasResults(query))
        {
            result.Query = query;
            return result;
        }

        // Reduce each keyword by one symbol until a result is found or all keywords are 3 symbols or less
        while (keywords.Any(keyword => keyword.Length > 3))
        {
            var keywordsToModify = new HashSet<string>(keywords.Where(keyword => keyword.Length > 3));

            foreach (var keyword in keywordsToModify)
            {
                // Reduce the keyword by one symbol
                var reducedKeyword = keyword[..^1];
                keywords.Remove(keyword);

                if (reducedKeyword.Length >= 3)
                    keywords.Add(reducedKeyword);
            }

            query = mainQuery.Clone();
            query.AddFullTextSearch(request, keywords);

            if (await HasResults(query))
            {
                result.Query = query;
                result.Keywords = keywords;
                return result;
            }

            query = mainQuery.Clone();
            query.AddLikeSearch(request, keywords);

            if (await HasResults(query))
            {
                result.Keywords = keywords;
                return result;
            }
        }

        result.Query = query;

        return result;
    }

    /// <summary>
    /// Escapes special characters in FTS5 keywords.
    /// </summary>
    /// <param name="keyword">The keyword to escape.</param>
    /// <returns>The escaped keyword.</returns>
    private static string EscapeFts5Keyword(string keyword)
    {
        return "\"" + keyword.Replace("\"", "\"\"") + "\"";
    }

    /// <summary>
    /// Adds a full-text search condition to the provided query using the specified keywords.
    /// </summary>
    /// <param name="query">The query to which the full-text search condition will be added.</param>
    /// <param name="request">The parameters for the full text search, including the search term, the columns to search in, and the pagination settings.</param>
    /// <param name="searchKeyWords">A set of keywords that will be used in the full-text search condition.</param>
    private static void AddFullTextSearch(this Query query, SearchHelperFullTextSearchRequest request, HashSet<string> searchKeyWords)
    {
        // Helper function to escape special characters in FTS5
        var escapedKeywords = searchKeyWords.Select(EscapeFts5Keyword);

        var ftsQuery = new Query(request.FtsTableName)
            .Select("Id", "rank")
            .WhereRaw($"{request.FtsTableName} MATCH ?", string.Join(" OR ", escapedKeywords));

        query.Join(ftsQuery.As("fts"), j => j.On($"{request.MainContentTableName}.{request.MainContentTablePrimaryKey}", "fts.Id"));

        query.OrderBy("fts.rank");
    }

    /// <summary>
    /// Adds a LIKE search condition to the given query for each combination of search column and search keyword.
    /// </summary>
    /// <param name="query">The query to add the LIKE search condition to.</param>
    /// <param name="request">The parameters for the full text search, including the search term, the columns to search in, and the pagination settings.</param>
    /// <param name="searchKeyWords">The keywords to search for.</param>
    private static void AddLikeSearch(this Query query, SearchHelperFullTextSearchRequest request, HashSet<string> searchKeyWords)
    {
        query.Join(request.FtsTableName, $"{request.MainContentTableName}.{request.MainContentTablePrimaryKey}", $"{request.FtsTableName}.{request.FtsPrimaryKey}");

        query.Where(q =>
        {
            foreach (var column in request.FtsSearchColumns)
            {
                foreach (var keyword in searchKeyWords)
                {
                    q.OrWhereRaw($"{column} LIKE ?", $"%{keyword}%");
                }
            }

            return q;
        });
    }

    /// <summary>
    /// Checks if the given search query has any results.
    /// </summary>
    /// <param name="searchQuery">The query to check for results.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the search query has any results.</returns>
    private static async Task<bool> HasResults(Query searchQuery)
    {
        return await searchQuery.ExistsAsync();
    }
}
