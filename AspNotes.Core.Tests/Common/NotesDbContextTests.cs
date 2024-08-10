using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Note.Models;
using SqlKata.Execution;

namespace AspNotes.Core.Tests.Common;

public class NotesDbContextTest : DatabaseTestBase
{

    [Fact]
    public async Task SaveChanges_UpdatesTimeStamps_WhenEntityIsAdded()
    {
        // Arrange
        var note = new NoteEntity
        {
            Title = "Test",
            Content = "Test",
            Section = "Section1"
        };
        var beforeAddition = DateTime.Now;

        // Act
        DbFixture.DbContext.Notes.Add(note);
        await DbFixture.DbContext.SaveChangesAsync();
        var afterAddition = DateTime.Now;

        // Assert
        Assert.True(note.CreatedAt >= beforeAddition && note.CreatedAt <= afterAddition, "CreatedAt should be within the expected range.");
        Assert.True(note.UpdatedAt >= beforeAddition && note.UpdatedAt <= afterAddition, "UpdatedAt should be within the expected range.");
    }

    [Fact]
    public async Task SaveChanges_UpdatesTimeStamps_WhenEntityIsUpdated()
    {
        // Arrange
        var existingNote = new NoteEntity
        {
            Title = "Original Title",
            Content = "Original Content",
            Section = "Original Section"
        };
        DbFixture.DbContext.Notes.Add(existingNote);
        await DbFixture.DbContext.SaveChangesAsync();

        var originalUpdatedAt = existingNote.UpdatedAt;

        existingNote.Title = "Updated Title";

        // Act
        await DbFixture.DbContext.SaveChangesAsync();

        // Assert
        Assert.True(existingNote.UpdatedAt > originalUpdatedAt, "UpdatedAt should be later than the original value after an update.");
    }

    [Fact]
    public async Task SaveChanges_DoesNotUpdateTimestamps_WhenEntityIsNotModified()
    {
        // Arrange
        var unmodifiedNote = new NoteEntity
        {
            Title = "Unchanged Title",
            Content = "Unchanged Content",
            Section = "Unchanged Section"
        };
        DbFixture.DbContext.Notes.Add(unmodifiedNote);
        await DbFixture.DbContext.SaveChangesAsync();

        var originalCreatedAt = unmodifiedNote.CreatedAt;
        var originalUpdatedAt = unmodifiedNote.UpdatedAt;

        // Act
        await DbFixture.DbContext.SaveChangesAsync(); // Attempting to save without making changes

        // Assert
        Assert.Equal(originalCreatedAt, unmodifiedNote.CreatedAt); // CreatedAt should remain unchanged
        Assert.Equal(originalUpdatedAt, unmodifiedNote.UpdatedAt); // UpdatedAt should remain unchanged, indicating no update occurred
    }

    [Fact]
    public async Task EntitySources_AreCorrectlySerializedAndDeserialized_WhenEntityIsAddedAndUpdated()
    {
        // Arrange
        var note = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "Test Section",
            Sources =
            [
                new NoteSource {
                    Id = "1",
                    Link = "http://example.com",
                    Title = "Example",
                    Description = "Example Description",
                    Image = "http://example.com/image.jpg",
                    ShowImage = true
                }
            ]
        };

        // Act - Add
        DbFixture.DbContext.Notes.Add(note);
        await DbFixture.DbContext.SaveChangesAsync();

        // Assert - After Add
        var addedNote = await DbFixture.DbContext.Notes.FindAsync(note.Id);
        Assert.NotNull(addedNote);
        Assert.NotNull(addedNote.Sources);
        Assert.Single(addedNote.Sources);
        Assert.Equal("http://example.com", addedNote.Sources.First().Link);
        Assert.Equal("Example", addedNote.Sources.First().Title);

        // Verify raw "Sources" value is valid JSON
        var n = new NotesTable("n");

        var rawSources = DbFixture.Db.Query()
            .Select(n.Sources)
            .From(n.GetFormattedTableName())
            .Where(n.Id, note.Id)
            .FirstOrDefault<string>();

        Assert.NotNull(rawSources);
        var parsedSources = rawSources.DeserializeJson<List<NoteSource>>();
        Assert.NotNull(parsedSources);
        Assert.Single(parsedSources);
        Assert.Equal("http://example.com", parsedSources.First().Link);

        // Act - Update
        addedNote.Sources.Add(new NoteSource
        {
            Id = "2",
            Link = "http://example2.com",
            Title = "Example 2",
            Description = "Example Description 2",
            Image = "http://example2.com/image.jpg",
            ShowImage = false
        });
        await DbFixture.DbContext.SaveChangesAsync();

        // Assert - After Update
        var updatedNote = await DbFixture.DbContext.Notes.FindAsync(note.Id);
        Assert.NotNull(updatedNote);
        Assert.NotNull(updatedNote.Sources);
        Assert.Equal(2, updatedNote.Sources.Count);
        Assert.Contains(updatedNote.Sources, s => s.Link == "http://example2.com" && s.Title == "Example 2");
    }

    [Fact]
    public async Task EntityTags_AreCorrectlyFormattedAndSerialized_WhenEntityIsAddedAndUpdated()
    {
        // Arrange
        var note = new NoteEntity
        {
            Title = "Test Note",
            Content = "Test Content",
            Section = "Test Section",
            Tags = ["tag1", "tag2", "tag3"]
        };

        // Act - Add
        DbFixture.DbContext.Notes.Add(note);
        await DbFixture.DbContext.SaveChangesAsync();

        // Assert - After Add
        var addedNote = await DbFixture.DbContext.Notes.FindAsync(note.Id);
        Assert.NotNull(addedNote);
        Assert.NotNull(addedNote.Tags);
        Assert.Equal(3, addedNote.Tags.Count);
        Assert.Contains("tag1", addedNote.Tags);
        Assert.Contains("tag2", addedNote.Tags);
        Assert.Contains("tag3", addedNote.Tags);

        // Verify raw "Tags" value is in correct format
        var n = new NotesTable("n");
        var rawTags = DbFixture.Db.Query()
            .Select(n.Tags)
            .From(n.GetFormattedTableName())
            .Where(n.Id, note.Id)
            .FirstOrDefault<string>();

        Assert.NotNull(rawTags);
        Assert.True(rawTags.Contains("[tag1]") && rawTags.Contains("[tag2]") && rawTags.Contains("[tag3]"), "Raw tags value is not in the expected format");

        // Act - Update
        addedNote.Tags = ["tag4", "tag5"];
        await DbFixture.DbContext.SaveChangesAsync();

        // Assert - After Update
        var updatedNote = await DbFixture.DbContext.Notes.FindAsync(note.Id);
        Assert.NotNull(updatedNote);
        Assert.NotNull(updatedNote.Tags);
        Assert.Equal(2, updatedNote.Tags.Count);
        Assert.Contains("tag4", updatedNote.Tags);
        Assert.Contains("tag5", updatedNote.Tags);

        // Verify updated raw "Tags" value is in correct format
        var updatedRawTags = DbFixture.Db.Query()
            .Select(n.Tags)
            .From(n.GetFormattedTableName())
            .Where(n.Id, note.Id)
            .FirstOrDefault<string>();

        Assert.NotNull(updatedRawTags);
        Assert.True(updatedRawTags.Contains("[tag4]") && updatedRawTags.Contains("[tag5]"), "Updated raw tags value is not in the expected format");
    }
}
