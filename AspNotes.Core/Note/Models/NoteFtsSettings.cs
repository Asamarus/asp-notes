namespace AspNotes.Core.Note.Models;

public class NoteFtsSettings
{
    public static readonly string FtsTitleColumn = "Title";

    public static readonly string FtsContentColumn = "Content";

    public static readonly HashSet<string> FtsSearchColumns = [FtsTitleColumn, FtsContentColumn];

    public static readonly string FtsTableName = "NotesFTS";

    public static readonly string MainContentTableName = "n";

    public static readonly string FtsPrimaryKey = "rowid";

    public static readonly string MainContentTablePrimaryKey = "Id";
}
