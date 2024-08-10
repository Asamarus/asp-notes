using AspNotes.Core.Note;
using AspNotes.Core.Note.Models;
using AspNotes.Core.Tag;
using AspNotes.Core.Tag.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNotes.Core.Tests.Tag;

public class TagsServiceTests : DatabaseTestBase
{
    private readonly TagsService _tagsService;
    private readonly NotesService _notesService;

    public TagsServiceTests()
    {
        var mockLogger = new Mock<ILogger<TagsService>>();
        _tagsService = new TagsService(DbFixture.DbContext, DbFixture.Db, mockLogger.Object);
        _notesService = new NotesService(DbFixture.DbContext, DbFixture.Db);
    }

    [Fact]
    public async Task UpdateNoteTags_AddsNewTags_WhenTagsAreNew()
    {
        // Arrange
        var note = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1"
        };

        DbFixture.DbContext.Notes.Add(note);
        await DbFixture.DbContext.SaveChangesAsync();

        var newTags = new HashSet<string> { "Tag1", "Tag2" };

        // Act
        var result = await _tagsService.UpdateNoteTags(note.Id, newTags);

        // Assert
        Assert.True(result);

        var tags = await DbFixture.DbContext.Tags.Where(x => x.Section == note.Section).ToListAsync();
        Assert.Equal(newTags.Count, tags.Count);
        Assert.All(newTags, tag => Assert.Contains(tags, x => x.Name == tag));

        var notesTags = await DbFixture.DbContext.NotesTags.Where(x => x.NoteId == note.Id).ToListAsync();
        Assert.Equal(newTags.Count, notesTags.Count);
        Assert.All(tags, tag => Assert.Contains(notesTags, x => x.TagId == tag.Id));

        var updatedNote = await DbFixture.DbContext.Notes.FindAsync(note.Id);
        Assert.NotNull(updatedNote);
        Assert.Equal(newTags.Count, updatedNote.Tags.Count);
        Assert.All(newTags, tag => Assert.Contains(updatedNote.Tags, x => x == tag));
    }

    [Fact]
    public async Task UpdateNoteTags_RemovesTags_WhenProvidedEmptyHashSetForExistingNote()
    {
        // Arrange
        var existingNote = new NoteEntity
        {
            Title = "Existing Note",
            Content = "Existing content",
            Section = "ExistingSection"
        };

        DbFixture.DbContext.Notes.Add(existingNote);

        await DbFixture.DbContext.SaveChangesAsync();

        var existingTags = new HashSet<string> { "ExistingTag1", "ExistingTag2" };
        await _tagsService.UpdateNoteTags(existingNote.Id, existingTags); // Initially add some tags

        var emptyTags = new HashSet<string>(); // Empty set for updating

        // Act
        var result = await _tagsService.UpdateNoteTags(existingNote.Id, emptyTags);

        // Assert
        Assert.True(result);

        // Ensure Tags table is empty for the note's section
        var tags = await DbFixture.DbContext.Tags.Where(x => x.Section == existingNote.Section).ToListAsync();
        Assert.Empty(tags);

        // Ensure NoteTags table is empty for the note
        var notesTags = await DbFixture.DbContext.NotesTags.Where(x => x.NoteId == existingNote.Id).ToListAsync();
        Assert.Empty(notesTags);

        // Ensure the updated note has an empty tags list
        var updatedNote = await DbFixture.DbContext.Notes.FindAsync(existingNote.Id);
        Assert.NotNull(updatedNote);
        Assert.Empty(updatedNote.Tags);
    }

    [Fact]
    public async Task UpdateNoteTags_UsesExistingTagCaseInsensitive_WhenNewTagMatchesExistingTagCaseInsensitive()
    {
        // Arrange
        var firstNote = new NoteEntity
        {
            Title = "First Note",
            Content = "First note content",
            Section = "SharedSection"
        };

        DbFixture.DbContext.Notes.Add(firstNote);

        var secondNote = new NoteEntity
        {
            Title = "Second Note",
            Content = "Second note content",
            Section = "SharedSection"
        };

        DbFixture.DbContext.Notes.Add(secondNote);

        await DbFixture.DbContext.SaveChangesAsync();

        // Add "newTag" to second note
        var existingTag = new HashSet<string> { "newTag" };
        await _tagsService.UpdateNoteTags(secondNote.Id, existingTag);

        // Act
        // Attempt to add "NewTag" to first note, expecting it to use the existing "newTag"
        var newTagForFirstNote = new HashSet<string> { "NewTag" };
        await _tagsService.UpdateNoteTags(firstNote.Id, newTagForFirstNote);

        // Assert
        // Check that "NewTag" was not added as a new entry, but "newTag" was reused
        var tags = await DbFixture.DbContext.Tags.Where(x => x.Section == firstNote.Section).ToListAsync();
        Assert.Single(tags); // Only one tag should exist because of case-insensitive match
        Assert.Contains(tags, x => x.Name == "newTag"); // The existing tag should be "newTag"

        // Check that both notes are linked to the same tag
        var firstNoteTags = await DbFixture.DbContext.NotesTags.Where(x => x.NoteId == firstNote.Id).ToListAsync();
        var secondNoteTags = await DbFixture.DbContext.NotesTags.Where(x => x.NoteId == secondNote.Id).ToListAsync();
        Assert.Single(firstNoteTags);
        Assert.Single(secondNoteTags);
        Assert.Equal(firstNoteTags.First().TagId, secondNoteTags.First().TagId); // Both notes should have the same TagId

        // Check that the tag name used in both notes is "newTag", not "NewTag"
        var updatedFirstNote = await DbFixture.DbContext.Notes.FindAsync(firstNote.Id);
        Assert.NotNull(updatedFirstNote);
        Assert.Contains(updatedFirstNote.Tags, x => x == "newTag");

        var updatedSecondNote = await DbFixture.DbContext.Notes.FindAsync(secondNote.Id);
        Assert.NotNull(updatedSecondNote);
        Assert.Contains(updatedSecondNote.Tags, x => x == "newTag");
    }

    [Fact]
    public async Task UpdateNoteTags_DeletesTagFromTagsTable_IfNotUsedInGivenSection()
    {
        // Arrange
        var section = "TestSection";

        var note1 = new NoteEntity
        {
            Title = "Note 1",
            Content = "Content 1",
            Section = section
        };

        DbFixture.DbContext.Notes.Add(note1);

        var note2 = new NoteEntity
        {
            Title = "Note 2",
            Content = "Content 2",
            Section = section
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();

        var sharedTag = "SharedTag";
        await _tagsService.UpdateNoteTags(note1.Id, [sharedTag]);
        await _tagsService.UpdateNoteTags(note2.Id, [sharedTag]);

        // Act
        // Remove the shared tag from note1
        await _tagsService.UpdateNoteTags(note1.Id, []);

        // Assert
        // The tag should still exist in the Tags table since it's used by note2
        var tagsAfterFirstRemoval = await DbFixture.DbContext.Tags.Where(x => x.Section == section).ToListAsync();
        Assert.Contains(tagsAfterFirstRemoval, t => t.Name == sharedTag);

        // Now remove the tag from note2 as well
        await _tagsService.UpdateNoteTags(note2.Id, new HashSet<string>());

        // The tag should be removed from the Tags table since it's no longer used in the section
        var tagsAfterSecondRemoval = await DbFixture.DbContext.Tags.Where(x => x.Section == section).ToListAsync();
        Assert.DoesNotContain(tagsAfterSecondRemoval, t => t.Name == sharedTag);

        // Ensure the tag is removed from the NotesTags table as well
        var notesTags = await DbFixture.DbContext.NotesTags.ToListAsync();
        Assert.Empty(notesTags);
    }

    [Fact]
    public async Task UpdateNoteTags_ReturnsFalse_IfNoteIdDoesNotExist()
    {
        // Arrange
        var nonExistentNoteId = 9999;
        var tagsToUpdate = new HashSet<string> { "Tag1", "Tag2" };

        // Act
        var result = await _tagsService.UpdateNoteTags(nonExistentNoteId, tagsToUpdate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateNoteTags_MakesNoChanges_IfNoteAlreadyHasProvidedTags()
    {
        // Arrange
        var note = new NoteEntity
        {
            Title = "Note with Tags",
            Content = "Content",
            Section = "SectionWithTags"
        };

        DbFixture.DbContext.Notes.Add(note);

        await DbFixture.DbContext.SaveChangesAsync();

        var existingTags = new HashSet<string> { "ExistingTag1", "ExistingTag2" };
        await _tagsService.UpdateNoteTags(note.Id, existingTags); // Initially add some tags

        // Capture the initial state before attempting to update with the same tags
        var initialTags = await DbFixture.DbContext.Tags.Where(x => x.Section == note.Section).ToListAsync();
        var initialNotesTags = await DbFixture.DbContext.NotesTags.Where(x => x.NoteId == note.Id).ToListAsync();

        // Act
        await _tagsService.UpdateNoteTags(note.Id, existingTags); // Attempt to update with the same tags

        // Assert
        // Ensure no new tags were added
        var tagsAfterUpdate = await DbFixture.DbContext.Tags.Where(x => x.Section == note.Section).ToListAsync();
        Assert.Equal(initialTags.Count, tagsAfterUpdate.Count);

        // Ensure no new NoteTags entries were created
        var notesTagsAfterUpdate = await DbFixture.DbContext.NotesTags.Where(x => x.NoteId == note.Id).ToListAsync();
        Assert.Equal(initialNotesTags.Count, notesTagsAfterUpdate.Count);

        // Ensure the Tags and NoteTags entries remain unchanged
        Assert.All(initialTags, tag => Assert.Contains(tagsAfterUpdate, x => x.Id == tag.Id && x.Name == tag.Name));
        Assert.All(initialNotesTags, noteTag => Assert.Contains(notesTagsAfterUpdate, x => x.TagId == noteTag.TagId && x.NoteId == noteTag.NoteId));
    }

    [Fact]
    public async Task UpdateNoteTags_SharedTagsAcrossNotes_EnsuresTagUniqueness()
    {
        // Arrange
        var section = "SharedTagsSection";

        var note1 = new NoteEntity
        {
            Title = "Note 1",
            Content = "Content for note 1",
            Section = section
        };

        DbFixture.DbContext.Notes.Add(note1);

        var note2 = new NoteEntity
        {
            Title = "Note 2",
            Content = "Content for note 2",
            Section = section
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();

        var sharedTagName = "SharedTag";
        var tagsForNote1 = new HashSet<string> { sharedTagName };
        var tagsForNote2 = new HashSet<string> { sharedTagName };

        // Act
        await _tagsService.UpdateNoteTags(note1.Id, tagsForNote1);
        await _tagsService.UpdateNoteTags(note2.Id, tagsForNote2);

        // Assert
        // Verify that only one tag exists in the database for the shared tag name
        var tags = await DbFixture.DbContext.Tags.Where(x => x.Section == section).ToListAsync();
        Assert.Single(tags);
        Assert.Equal(sharedTagName, tags.First().Name);

        // Verify that both notes are linked to the same tag
        var note1Tags = await DbFixture.DbContext.NotesTags.Where(x => x.NoteId == note1.Id).ToListAsync();
        var note2Tags = await DbFixture.DbContext.NotesTags.Where(x => x.NoteId == note2.Id).ToListAsync();
        Assert.Single(note1Tags);
        Assert.Single(note2Tags);
        Assert.Equal(note1Tags.First().TagId, note2Tags.First().TagId);
    }

    [Fact]
    public async Task Autocomplete_ReturnsCorrectTags_WhenSearchTermMatches()
    {
        // Arrange
        var searchTerm = "Tag";
        await DbFixture.DbContext.Tags.AddRangeAsync(new TagEntity
        {
            Name = "Tag One",
            Section = "section1"
        }, new TagEntity
        {
            Name = "Another Tag",
            Section = "section2"
        });
        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var results = await _tagsService.Autocomplete(searchTerm);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.Name == "Tag One");
        Assert.Contains(results, r => r.Name == "Another Tag");
    }

    [Fact]
    public async Task Autocomplete_ReturnsTagsFromSpecificSection_WhenSectionIsSpecified()
    {
        // Arrange
        var searchTerm = "Tag";
        var specificSection = "section1";
        await DbFixture.DbContext.Tags.AddRangeAsync(new TagEntity
        {
            Name = "Tag One",
            Section = "section1"
        }, new TagEntity
        {
            Name = "Another Tag",
            Section = "section2"
        });
        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var results = await _tagsService.Autocomplete(searchTerm, specificSection);

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.All(results, r => Assert.NotEqual("Another Tag", r.Name));
    }

    [Fact]
    public async Task Autocomplete_ReturnsEmptyList_WhenNoTagsMatch()
    {
        // Arrange
        var searchTerm = "Nonexistent";

        // Act
        var results = await _tagsService.Autocomplete(searchTerm);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetTags_ReturnsAllTags_WhenSectionIsNull()
    {
        // Arrange
        var note = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1"
        };

        DbFixture.DbContext.Notes.Add(note);

        await DbFixture.DbContext.SaveChangesAsync();

        var newTags = new HashSet<string> { "Tag1", "Tag2" };
        await _tagsService.UpdateNoteTags(note.Id, newTags);

        // Act
        var results = await _tagsService.GetTags(null);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task GetTags_ReturnsTagsFromSpecificSection_WhenSectionIsProvided()
    {
        // Arrange
        var section = "section1";

        var note1 = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = section
        };

        DbFixture.DbContext.Notes.Add(note1);

        await DbFixture.DbContext.SaveChangesAsync();

        var newTagsSection1 = new HashSet<string> { "Tag1", "Tag2" };
        await _tagsService.UpdateNoteTags(note1.Id, newTagsSection1);

        var note2 = new NoteEntity
        {
            Title = "Another Note",
            Content = "Some more text",
            Section = section
        };

        DbFixture.DbContext.Notes.Add(note2);

        await DbFixture.DbContext.SaveChangesAsync();

        await _tagsService.UpdateNoteTags(note2.Id, ["Tag2"]);

        var note3 = new NoteEntity
        {
            Title = "Another Note",
            Content = "Some more text",
            Section = "section2"
        };

        DbFixture.DbContext.Notes.Add(note3);

        await DbFixture.DbContext.SaveChangesAsync();

        var newTagsSection2 = new HashSet<string> { "Tag3" };
        await _tagsService.UpdateNoteTags(note3.Id, newTagsSection2);

        // Act
        var results = await _tagsService.GetTags(section);

        // Assert
        var expectedResults = new List<TagsServiceGetTagsResultItem>
        {
            new() { Id = 1, Name = "Tag1", Count = 1 },
            new() { Id = 2, Name = "Tag2", Count = 2 }
        };

        results.Should().BeEquivalentTo(expectedResults);
    }
}
