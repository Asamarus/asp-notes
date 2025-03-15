using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNotes.Tests.Services;

public class TagsServiceTests : InMemoryDatabaseTestBase
{
    private readonly TagsService tagsService;
    private readonly Mock<ILogger<TagsService>> loggerMock;

    public TagsServiceTests()
    {
        loggerMock = new Mock<ILogger<TagsService>>();
        tagsService = new TagsService(DbContext, loggerMock.Object);
    }

    [Fact]
    public async Task UpdateNoteTags_ShouldReturnFalse_WhenNoteDoesNotExist()
    {
        // Arrange
        var noteId = 9999;
        var newTags = new HashSet<string> { "Tag1", "Tag2" };

        // Act
        var result = await tagsService.UpdateNoteTags(noteId, newTags, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateNoteTags_ShouldReturnTrue_WhenTagsAreSame()
    {
        // Arrange
        var note = new NoteEntity { Section = "Test", TagsList = ["Tag1", "Tag2"] };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        var newTags = new HashSet<string> { "Tag1", "Tag2" };

        // Act
        var result = await tagsService.UpdateNoteTags(note.Id, newTags, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateNoteTags_ShouldUpdateTags_WhenTagsAreDifferent()
    {
        // Arrange
        var note = new NoteEntity { Section = "Test", TagsList = ["OldTag"] };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        var newTags = new HashSet<string> { "NewTag" };

        // Act
        var result = await tagsService.UpdateNoteTags(note.Id, newTags, CancellationToken.None);
        var updatedNote = await DbContext.Notes.FindAsync(note.Id);

        // Assert
        Assert.True(result);
        Assert.NotNull(updatedNote);
        Assert.Contains("NewTag", updatedNote.TagsList);
    }

    [Fact]
    public async Task UpdateNoteTags_ShouldAddNewTags_WhenTagsAreNew()
    {
        // Arrange
        var note = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1"
        };

        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        var newTags = new HashSet<string> { "Tag1", "Tag2" };

        // Act
        var result = await tagsService.UpdateNoteTags(note.Id, newTags, CancellationToken.None);

        // Assert
        Assert.True(result);

        var tags = await DbContext.Tags.Where(x => x.Section == note.Section).ToListAsync();
        Assert.Equal(newTags.Count, tags.Count);
        Assert.All(newTags, tag => Assert.Contains(tags, x => x.Name == tag));

        var notesTags = await DbContext.NotesTags.Where(x => x.NoteId == note.Id).ToListAsync();
        Assert.Equal(newTags.Count, notesTags.Count);
        Assert.All(tags, tag => Assert.Contains(notesTags, x => x.TagId == tag.Id));

        var updatedNote = await DbContext.Notes.FindAsync(note.Id);
        Assert.NotNull(updatedNote);
        Assert.Equal(newTags.Count, updatedNote.TagsList.Count);
        Assert.All(newTags, tag => Assert.Contains(updatedNote.TagsList, x => x == tag));
    }

    [Fact]
    public async Task UpdateNoteTags_ShouldRemoveTags_WhenProvidedEmptyHashSetForExistingNote()
    {
        // Arrange
        var existingNote = new NoteEntity
        {
            Title = "Existing Note",
            Content = "Existing content",
            Section = "ExistingSection"
        };

        DbContext.Notes.Add(existingNote);

        await DbContext.SaveChangesAsync();

        var existingTags = new HashSet<string> { "ExistingTag1", "ExistingTag2" };
        await tagsService.UpdateNoteTags(existingNote.Id, existingTags, CancellationToken.None); // Initially add some tags

        var emptyTags = new HashSet<string>(); // Empty set for updating

        // Act
        var result = await tagsService.UpdateNoteTags(existingNote.Id, emptyTags, CancellationToken.None);

        // Assert
        Assert.True(result);

        // Ensure Tags table is empty for the note's section
        var tags = await DbContext.Tags.Where(x => x.Section == existingNote.Section).ToListAsync();
        Assert.Empty(tags);

        // Ensure NoteTags table is empty for the note
        var notesTags = await DbContext.NotesTags.Where(x => x.NoteId == existingNote.Id).ToListAsync();
        Assert.Empty(notesTags);

        // Ensure the updated note has an empty tags list
        var updatedNote = await DbContext.Notes.FindAsync(existingNote.Id);
        Assert.NotNull(updatedNote);
        Assert.Empty(updatedNote.Tags);
    }

    [Fact]
    public async Task UpdateNoteTags_ShouldUseExistingTagCaseInsensitive_WhenNewTagMatchesExistingTagCaseInsensitive()
    {
        // Arrange
        var firstNote = new NoteEntity
        {
            Title = "First Note",
            Content = "First note content",
            Section = "SharedSection"
        };

        DbContext.Notes.Add(firstNote);

        var secondNote = new NoteEntity
        {
            Title = "Second Note",
            Content = "Second note content",
            Section = "SharedSection"
        };

        DbContext.Notes.Add(secondNote);

        await DbContext.SaveChangesAsync();

        // Add "newTag" to second note
        var existingTag = new HashSet<string> { "newTag" };
        await tagsService.UpdateNoteTags(secondNote.Id, existingTag, CancellationToken.None);

        // Act
        // Attempt to add "NewTag" to first note, expecting it to use the existing "newTag"
        var newTagForFirstNote = new HashSet<string> { "NewTag" };
        await tagsService.UpdateNoteTags(firstNote.Id, newTagForFirstNote, CancellationToken.None);

        // Assert
        // Check that "NewTag" was not added as a new entry, but "newTag" was reused
        var tags = await DbContext.Tags.Where(x => x.Section == firstNote.Section).ToListAsync();
        Assert.Single(tags); // Only one tag should exist because of case-insensitive match
        Assert.Contains(tags, x => x.Name == "newTag"); // The existing tag should be "newTag"

        // Check that both notes are linked to the same tag
        var firstNoteTags = await DbContext.NotesTags.Where(x => x.NoteId == firstNote.Id).ToListAsync();
        var secondNoteTags = await DbContext.NotesTags.Where(x => x.NoteId == secondNote.Id).ToListAsync();
        Assert.Single(firstNoteTags);
        Assert.Single(secondNoteTags);
        Assert.Equal(firstNoteTags[0].TagId, secondNoteTags[0].TagId); // Both notes should have the same TagId

        // Check that the tag name used in both notes is "newTag", not "NewTag"
        var updatedFirstNote = await DbContext.Notes.FindAsync(firstNote.Id);
        Assert.NotNull(updatedFirstNote);
        Assert.Contains(updatedFirstNote.TagsList, x => x == "newTag");

        var updatedSecondNote = await DbContext.Notes.FindAsync(secondNote.Id);
        Assert.NotNull(updatedSecondNote);
        Assert.Contains(updatedSecondNote.TagsList, x => x == "newTag");
    }

    [Fact]
    public async Task UpdateNoteTags_ShouldDeleteTagFromTagsTable_IfNotUsedInGivenSection()
    {
        // Arrange
        var section = "TestSection";

        var note1 = new NoteEntity
        {
            Title = "Note 1",
            Content = "Content 1",
            Section = section
        };

        DbContext.Notes.Add(note1);

        var note2 = new NoteEntity
        {
            Title = "Note 2",
            Content = "Content 2",
            Section = section
        };

        DbContext.Notes.Add(note2);

        await DbContext.SaveChangesAsync();

        var sharedTag = "SharedTag";
        await tagsService.UpdateNoteTags(note1.Id, [sharedTag], CancellationToken.None);
        await tagsService.UpdateNoteTags(note2.Id, [sharedTag], CancellationToken.None);

        // Act
        // Remove the shared tag from note1
        await tagsService.UpdateNoteTags(note1.Id, [], CancellationToken.None);

        // Assert
        // The tag should still exist in the Tags table since it's used by note2
        var tagsAfterFirstRemoval = await DbContext.Tags.Where(x => x.Section == section).ToListAsync();
        Assert.Contains(tagsAfterFirstRemoval, t => t.Name == sharedTag);

        // Now remove the tag from note2 as well
        await tagsService.UpdateNoteTags(note2.Id, [], CancellationToken.None);

        // The tag should be removed from the Tags table since it's no longer used in the section
        var tagsAfterSecondRemoval = await DbContext.Tags.Where(x => x.Section == section).ToListAsync();
        Assert.DoesNotContain(tagsAfterSecondRemoval, t => t.Name == sharedTag);

        // Ensure the tag is removed from the NotesTags table as well
        var notesTags = await DbContext.NotesTags.ToListAsync();
        Assert.Empty(notesTags);
    }

    [Fact]
    public async Task UpdateNoteTags_ShouldMakeNoChanges_IfNoteAlreadyHasProvidedTags()
    {
        // Arrange
        var note = new NoteEntity
        {
            Title = "Note with Tags",
            Content = "Content",
            Section = "SectionWithTags"
        };

        DbContext.Notes.Add(note);

        await DbContext.SaveChangesAsync();

        var existingTags = new HashSet<string> { "ExistingTag1", "ExistingTag2" };
        await tagsService.UpdateNoteTags(note.Id, existingTags, CancellationToken.None); // Initially add some tags

        // Capture the initial state before attempting to update with the same tags
        var initialTags = await DbContext.Tags.Where(x => x.Section == note.Section).ToListAsync();
        var initialNotesTags = await DbContext.NotesTags.Where(x => x.NoteId == note.Id).ToListAsync();

        // Act
        await tagsService.UpdateNoteTags(note.Id, existingTags, CancellationToken.None); // Attempt to update with the same tags

        // Assert
        // Ensure no new tags were added
        var tagsAfterUpdate = await DbContext.Tags.Where(x => x.Section == note.Section).ToListAsync();
        Assert.Equal(initialTags.Count, tagsAfterUpdate.Count);

        // Ensure no new NoteTags entries were created
        var notesTagsAfterUpdate = await DbContext.NotesTags.Where(x => x.NoteId == note.Id).ToListAsync();
        Assert.Equal(initialNotesTags.Count, notesTagsAfterUpdate.Count);

        // Ensure the Tags and NoteTags entries remain unchanged
        Assert.All(initialTags, tag => Assert.Contains(tagsAfterUpdate, x => x.Id == tag.Id && x.Name == tag.Name));
        Assert.All(initialNotesTags, noteTag => Assert.Contains(notesTagsAfterUpdate, x => x.TagId == noteTag.TagId && x.NoteId == noteTag.NoteId));
    }

    [Fact]
    public async Task UpdateNoteTags_ShouldShareTagsAcrossNotes_EnsuresTagUniqueness()
    {
        // Arrange
        var section = "SharedTagsSection";

        var note1 = new NoteEntity
        {
            Title = "Note 1",
            Content = "Content for note 1",
            Section = section
        };

        DbContext.Notes.Add(note1);

        var note2 = new NoteEntity
        {
            Title = "Note 2",
            Content = "Content for note 2",
            Section = section
        };

        DbContext.Notes.Add(note2);

        await DbContext.SaveChangesAsync();

        var sharedTagName = "SharedTag";
        var tagsForNote1 = new HashSet<string> { sharedTagName };
        var tagsForNote2 = new HashSet<string> { sharedTagName };

        // Act
        await tagsService.UpdateNoteTags(note1.Id, tagsForNote1, CancellationToken.None);
        await tagsService.UpdateNoteTags(note2.Id, tagsForNote2, CancellationToken.None);

        // Assert
        // Verify that only one tag exists in the database for the shared tag name
        var tags = await DbContext.Tags.Where(x => x.Section == section).ToListAsync();
        Assert.Single(tags);
        Assert.Equal(sharedTagName, tags[0].Name);

        // Verify that both notes are linked to the same tag
        var note1Tags = await DbContext.NotesTags.Where(x => x.NoteId == note1.Id).ToListAsync();
        var note2Tags = await DbContext.NotesTags.Where(x => x.NoteId == note2.Id).ToListAsync();
        Assert.Single(note1Tags);
        Assert.Single(note2Tags);
        Assert.Equal(note1Tags[0].TagId, note2Tags[0].TagId);
    }

    [Fact]
    public async Task Autocomplete_ShouldReturnMatchingTags()
    {
        // Arrange
        var tag1 = new TagEntity { Name = "TagOne", Section = "Test" };
        var tag2 = new TagEntity { Name = "TagTwo", Section = "Test" };
        DbContext.Tags.AddRange(tag1, tag2);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await tagsService.Autocomplete("Tag", "Test", CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, t => t.Name == "TagOne");
        Assert.Contains(result, t => t.Name == "TagTwo");
    }

    [Fact]
    public async Task Autocomplete_ShouldReturnTagsFromSpecificSection_WhenSectionIsSpecified()
    {
        // Arrange
        var searchTerm = "Tag";
        var specificSection = "section1";
        await DbContext.Tags.AddRangeAsync(new TagEntity
        {
            Name = "Tag One",
            Section = "section1"
        }, new TagEntity
        {
            Name = "Another Tag",
            Section = "section2"
        });
        await DbContext.SaveChangesAsync();

        // Act
        var results = await tagsService.Autocomplete(searchTerm, specificSection, CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.All(results, r => Assert.NotEqual("Another Tag", r.Name));
    }

    [Fact]
    public async Task Autocomplete_ShouldReturnEmptyList_WhenNoTagsMatch()
    {
        // Arrange
        var searchTerm = "Nonexistent";

        // Act
        var results = await tagsService.Autocomplete(searchTerm, null, CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetTags_ShouldReturnTagsWithCount()
    {
        // Arrange
        var note = new NoteEntity { Section = "Test" };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();
        await tagsService.UpdateNoteTags(note.Id, ["TagOne"], CancellationToken.None);

        // Act
        var result = await tagsService.GetTags("Test", CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, t => t.Name == "TagOne" && t.Count == 1);
    }

    [Fact]
    public async Task GetTags_ShouldReturnAllTags_WhenSectionIsNull()
    {
        // Arrange
        var note = new NoteEntity
        {
            Title = "Note title",
            Content = "Some text",
            Section = "section1"
        };

        DbContext.Notes.Add(note);

        await DbContext.SaveChangesAsync();

        var newTags = new HashSet<string> { "Tag1", "Tag2" };
        await tagsService.UpdateNoteTags(note.Id, newTags, CancellationToken.None);

        // Act
        var results = await tagsService.GetTags(null, CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }
}
