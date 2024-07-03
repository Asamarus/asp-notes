using AspNotes.Core.Common.Models;

namespace AspNotes.Core.Section.Models;

public class SectionsTable : DbTable
{
    public override string TableName => "Sections";

    public string Id { get; private set; }

    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public string Color { get; private set; }

    public string Position { get; private set; }

    public SectionsTable(string? alias = null) : base(alias)
    {
        alias ??= TableName;

        TableAlias = alias;

        Id = alias + "." + nameof(SectionEntity.Id);

        Name = alias + "." + nameof(SectionEntity.Name);

        DisplayName = alias + "." + nameof(SectionEntity.DisplayName);

        Color = alias + "." + nameof(SectionEntity.Color);

        Position = alias + "." + nameof(SectionEntity.Position);
    }
}
