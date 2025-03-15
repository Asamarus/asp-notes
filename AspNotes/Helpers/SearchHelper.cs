using System.Text.RegularExpressions;
using System.Web;

namespace AspNotes.Helpers;

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

        // trick to save spaces for words in separate html tags
        // example: <p>Hello</p><p>world</p> will be 'Hello world' and not 'Helloworld' in search index
        searchIndex = searchIndex.Replace("<", " <");
        searchIndex = HtmlHelper.StripTags(searchIndex);
        searchIndex = MultipleSpaceRegex().Replace(searchIndex, " ");

        searchIndex = HttpUtility.HtmlDecode(searchIndex);

        if (toLower)
        {
            searchIndex = searchIndex.ToLower();
        }

        if (limit > 0 && limit < searchIndex.Length)
        {
            searchIndex = searchIndex[..(limit + 1)].Trim() + "...";
        }

        return searchIndex.Trim();
    }

    /// <summary>
    /// Generates a search snippet from the provided text based on a list of keywords.
    /// </summary>
    /// <param name="keywords">A list of keywords to search for in the text.</param>
    /// <param name="text">The text to search in.</param>
    /// <param name="limit">The maximum number of snippets to return.</param>
    /// <param name="foundWholePhrase">A flag indicating whether the whole phrase was found in the text.</param>
    /// <param name="highlight">A flag indicating whether to highlight the keywords in the returned snippets.</param>
    /// <returns>A string that represents the search snippet. If no snippets are found, an empty string is returned.</returns>
    public static string GetSearchSnippet(
        HashSet<string> keywords,
        string text,
        int limit = 10,
        bool foundWholePhrase = false,
        bool highlight = false)
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
                    {
                        newKeywords.Add(word);
                    }
                }
            }

            keywords = newKeywords;
        }

        var result = new List<string>();
        var regexDict = new Dictionary<string, (Regex MatchRegex, Regex? ReplaceRegex)>();

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

            var matches = value.MatchRegex.Matches(text);

            if (matches.Count != 0)
            {
                foreach (Match match in matches)
                {
                    var trimmedMatch = match.Value.Trim();

                    if (string.IsNullOrEmpty(trimmedMatch))
                    {
                        continue;
                    }

                    if (snippetCurrent >= limit)
                    {
                        return string.Join(" ", result);
                    }

                    if (highlight && value.ReplaceRegex != null)
                    {
                        trimmedMatch = value.ReplaceRegex.Replace(trimmedMatch, "<em>$1</em>");
                    }

                    result.Add($"{trimmedMatch}...");

                    snippetCurrent++;
                }
            }
        }

        return string.Join(" ", result);
    }

    /// <summary>
    /// Generates a Regex object that matches multiple spaces.
    /// </summary>
    /// <returns>A Regex object that matches multiple spaces.</returns>
    [GeneratedRegex("  +")]
    private static partial Regex MultipleSpaceRegex();
}
