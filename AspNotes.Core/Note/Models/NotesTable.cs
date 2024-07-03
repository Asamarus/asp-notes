using AspNotes.Core.Common.Models;

namespace AspNotes.Core.Note.Models;

public class NotesTable : DbTable
{
    public override string TableName => "Notes";

    public string Id { get; private set; }

    public string Title { get; private set; }

    public string TitleSearchIndex { get; private set; }

    public string Section { get; private set; }

    public string Content { get; private set; }

    public string ContentSearchIndex { get; private set; }

    public string Preview { get; private set; }

    public string Book { get; private set; }

    public string Tags { get; private set; }

    public string Sources { get; private set; }

    public NotesTable(string? alias = null) : base(alias)
    {
        alias ??= TableName;

        TableAlias = alias;

        Id = alias + "." + nameof(NoteEntity.Id);

        Title = alias + "." + nameof(NoteEntity.Title);

        TitleSearchIndex = alias + "." + nameof(NoteEntity.TitleSearchIndex);

        Section = alias + "." + nameof(NoteEntity.Section);

        Content = alias + "." + nameof(NoteEntity.Content);

        ContentSearchIndex = alias + "." + nameof(NoteEntity.ContentSearchIndex);

        Preview = alias + "." + nameof(NoteEntity.Preview);

        Book = alias + "." + nameof(NoteEntity.Book);

        Tags = alias + "." + nameof(NoteEntity.Tags);

        Sources = alias + "." + nameof(NoteEntity.Sources);
    }
}
