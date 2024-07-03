using AspNotes.Core.Common.Models;

namespace AspNotes.Core.Book.Models;

public class BooksTable : DbTable
{
    public override string TableName => "Books";

    public string Id { get; private set; }

    public string Section { get; private set; }

    public string Name { get; private set; }

    public BooksTable(string? alias = null) : base(alias)
    {
        alias ??= TableName;

        TableAlias = alias;

        Id = alias + "." + nameof(BookEntity.Id);

        Section = alias + "." + nameof(BookEntity.Section);

        Name = alias + "." + nameof(BookEntity.Name);
    }
}