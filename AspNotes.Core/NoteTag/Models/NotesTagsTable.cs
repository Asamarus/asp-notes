using AspNotes.Core.Common.Models;

namespace AspNotes.Core.NoteTag.Models;

public class NotesTagsTable : DbTable
{
    public override string TableName => "NotesTags";

    public string Id { get; private set; }

    public string NoteId { get; private set; }

    public string TagId { get; private set; }

    public NotesTagsTable(string? alias = null) : base(alias)
    {
        alias ??= TableName;

        TableAlias = alias;

        Id = alias + "." + nameof(NoteTagEntity.Id);

        NoteId = alias + "." + nameof(NoteTagEntity.NoteId);

        TagId = alias + "." + nameof(NoteTagEntity.TagId);
    }
}
