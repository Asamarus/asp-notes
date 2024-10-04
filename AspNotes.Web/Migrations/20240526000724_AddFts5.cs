using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNotes.Web.Migrations;

/// <inheritdoc />
public partial class AddFts5 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"CREATE VIRTUAL TABLE IF NOT EXISTS NotesFTS USING fts5(Id, Title, Content, tokenize = 'trigram');");

        //Triggers to update FTS table
        migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS NotesFTSAfterInsert;");
        migrationBuilder.Sql(@"CREATE TRIGGER NotesFTSAfterInsert AFTER INSERT on Notes
            FOR EACH ROW
            BEGIN
            INSERT INTO NotesFTS (Id, Title, Content) VALUES (NEW.Id, NEW.TitleSearchIndex, NEW.ContentSearchIndex);
            END");

        migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS NotesFTSAfterUpdate;");
        migrationBuilder.Sql(@"CREATE TRIGGER NotesFTSAfterUpdate AFTER UPDATE on Notes
            FOR EACH ROW
            WHEN OLD.TitleSearchIndex IS DISTINCT FROM NEW.TitleSearchIndex OR OLD.ContentSearchIndex IS DISTINCT FROM NEW.ContentSearchIndex
            BEGIN
            UPDATE NotesFTS SET Title = NEW.TitleSearchIndex, Content = NEW.ContentSearchIndex WHERE Id = NEW.Id;
            END");

        migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS NotesFTSAfterDelete;");
        migrationBuilder.Sql(@"CREATE TRIGGER NotesFTSAfterDelete AFTER DELETE on Notes
            FOR EACH ROW
            BEGIN
            DELETE FROM NotesFTS WHERE Id = OLD.Id;
            END");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TABLE IF EXISTS NotesFTS;");

        migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS NotesFTSAfterInsert;");
        migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS NotesFTSAfterUpdate;");
        migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS NotesFTSAfterDelete;");
    }
}
