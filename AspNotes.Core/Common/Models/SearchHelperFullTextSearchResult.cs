using SqlKata;

namespace AspNotes.Core.Common.Models;

public class SearchHelperFullTextSearchResult
{
    public required Query Query { get; set; }

    public bool FoundWholePhrase { get; set; } = false;

    public required HashSet<string> Keywords { get; set; }
}
