using AspNotes.Core.Common.Models;

namespace AspNotes.Core.Tag.Models;

public class TagsTable : DbTable
{
    public override string TableName => "Tags";

    public string Id { get; private set; }

    public string Section { get; private set; }

    public string Name { get; private set; }

    public TagsTable(string? alias = null) : base(alias)
    {
        alias ??= TableName;

        TableAlias = alias;

        Id = alias + "." + nameof(TagEntity.Id);

        Section = alias + "." + nameof(TagEntity.Section);

        Name = alias + "." + nameof(TagEntity.Name);
    }
}
