using AspNotes.Common;
using AspNotes.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNotes.Helpers;

/// <summary>
/// Provides helper methods for searching notes.
/// </summary>
public static class NotesSearchHelper
{
    /// <summary>
    /// Performs a full-text search on the notes.
    /// </summary>
    /// <param name="mainQuery">The main query to search within.</param>
    /// <param name="searchTerm">The search term to use.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A tuple containing the query, keywords, and a flag indicating if the whole phrase was found.</returns>
    public static async Task<(IQueryable<NoteEntity> Query, HashSet<string> Keywords, bool FoundWholePhrase)> FullTextSearch(
        IQueryable<NoteEntity> mainQuery,
        string searchTerm,
        CancellationToken cancellationToken)
    {
        (IQueryable<NoteEntity> Query, HashSet<string> Keywords, bool FoundWholePhrase) result =
        (
            mainQuery,
            new HashSet<string> { searchTerm },
            false);

        // check if whole phrase is found
        var query = AddLikeSearch(mainQuery, [searchTerm]);

        if (await query.AnyAsync(cancellationToken))
        {
            result.Query = query;
            result.FoundWholePhrase = true;
            return result;
        }

        var keywords = searchTerm.Split(' ')
               .Where(word => word.Length >= 3)
               .ToHashSet();

        result.Keywords = keywords;

        // try FTS
        query = AddFullTextSearch(mainQuery, [searchTerm]);

        if (await query.AnyAsync(cancellationToken))
        {
            result.Query = query;
            return result;
        }

        query = AddFullTextSearch(mainQuery, keywords);

        if (await query.AnyAsync(cancellationToken))
        {
            result.Query = query;
            return result;
        }

        query = AddLikeSearch(mainQuery, keywords);

        if (await query.AnyAsync(cancellationToken))
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
                {
                    keywords.Add(reducedKeyword);
                }
            }

            query = AddFullTextSearch(mainQuery, keywords);

            if (await query.AnyAsync(cancellationToken))
            {
                result.Query = query;
                result.Keywords = keywords;
                return result;
            }

            query = AddLikeSearch(mainQuery, keywords);

            if (await query.AnyAsync(cancellationToken))
            {
                result.Query = query;
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
    /// Adds full-text search conditions to the query.
    /// </summary>
    /// <param name="query">The query to modify.</param>
    /// <param name="searchKeyWords">The keywords to search for.</param>
    /// <returns>The modified query.</returns>
    private static IQueryable<NoteEntity> AddFullTextSearch(
        IQueryable<NoteEntity> query,
        HashSet<string> searchKeyWords)
    {
        var escapedKeywords = searchKeyWords.Select(EscapeFts5Keyword);

        query = query.Include(n => n.NoteFts)
            .Where(n => n.NoteFts.Match == string.Join(" OR ", escapedKeywords))
            .OrderBy(n => n.NoteFts.Rank);

        return query;
    }

    /// <summary>
    /// Adds LIKE search conditions to the query.
    /// </summary>
    /// <param name="query">The query to modify.</param>
    /// <param name="searchKeyWords">The keywords to search for.</param>
    /// <returns>The modified query.</returns>
    private static IQueryable<NoteEntity> AddLikeSearch(
        IQueryable<NoteEntity> query,
        HashSet<string> searchKeyWords)
    {
        var predicate = PredicateBuilder.False<NoteEntity>();

        foreach (var keyword in searchKeyWords)
        {
            predicate = predicate.Or(n => EF.Functions.Like(n.NoteFts.Title, $"%{keyword}%"));
            predicate = predicate.Or(n => EF.Functions.Like(n.NoteFts.Content, $"%{keyword}%"));
        }

        query = query.Include(n => n.NoteFts).Where(predicate).OrderBy(n => n.CreatedAt);

        return query;
    }
}
