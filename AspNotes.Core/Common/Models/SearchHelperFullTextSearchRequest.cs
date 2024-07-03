using SqlKata;

namespace AspNotes.Core.Common.Models;

public class SearchHelperFullTextSearchRequest
{
    public required Query Query { get; set; }

    private HashSet<string> _ftsSearchColumns = [];

    public HashSet<string> FtsSearchColumns
    {
        get => _ftsSearchColumns.Select(column => $"{FtsTableName}.{column}").ToHashSet();
        set => _ftsSearchColumns = value;
    }

    public required string SearchTerm { get; set; }

    public required string FtsTableName { get; set; }

    public required string FtsPrimaryKey { get; set; }

    public required string MainContentTableName { get; set; }

    public required string MainContentTablePrimaryKey { get; set; }
}
