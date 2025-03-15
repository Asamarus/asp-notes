using AspNotes.Entities;
using AspNotes.Interfaces;
using AspNotes.Services;
using Moq;

namespace AspNotes.Tests.Services;
public class SectionsServiceTests : InMemoryDatabaseTestBase
{
    private readonly SectionsService sectionsService;
    private readonly Mock<IAppCache> appCacheMock;

    public SectionsServiceTests()
    {
        appCacheMock = new Mock<IAppCache>();
        sectionsService = new SectionsService(DbContext, appCacheMock.Object);
    }

    [Fact]
    public async Task IsSectionNameUnique_ShouldReturnTrue_WhenNameIsUnique()
    {
        // Arrange
        var sectionName = "UniqueSection";

        // Act
        var result = await sectionsService.IsSectionNameUnique(sectionName, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSectionNameUnique_ShouldReturnFalse_WhenNameIsNotUnique()
    {
        // Arrange
        var sectionName = "ExistingSection";
        DbContext.Sections.Add(new SectionEntity { 
            Name = sectionName,
            DisplayName = "Existing Section",
            Color = "#000000",
            Position = 0
        });
        await DbContext.SaveChangesAsync();

        // Act
        var result = await sectionsService.IsSectionNameUnique(sectionName, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsSectionIdPresent_ShouldReturnFalse_WhenIdDoesNotExist()
    {
        // Arrange

        // Act
        var result = await sectionsService.IsSectionIdPresent(9999, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsSectionIdPresent_ShouldReturnTrue_WhenIdExists()
    {
        // Arrange
        var sectionId = await sectionsService.CreateSection("Test", "Test", "#000000", CancellationToken.None);

        // Act
        var result = await sectionsService.IsSectionIdPresent(sectionId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSectionNameValid_ShouldReturnFalse_WhenNameIsInvalid()
    {
        // Arrange

        // Act
        var result = await sectionsService.IsSectionNameValid("Test", CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsSectionNameValid_ShouldReturnTrue_WhenNameIsValid()
    {
        // Arrange
        await sectionsService.CreateSection("Test", "Test", "#000000", CancellationToken.None);

        // Act
        var result = await sectionsService.IsSectionNameValid("Test", CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSectionHavingNotes_ShouldReturnTrue_WhenSectionIsHavingNotes()
    {
        // Arrange
        await sectionsService.CreateSection("Test", "Test", "#000000", CancellationToken.None);

        var note = new NoteEntity
        {
            Section = "Test",
        };
        DbContext.Notes.Add(note);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await sectionsService.IsSectionHavingNotes(1, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSectionHavingNotes_ShouldReturnFalse_WhenSectionIsNotHavingNotes()
    {
        // Arrange
        await sectionsService.CreateSection("Test", "Test", "#000000", CancellationToken.None);

        // Act
        var result = await sectionsService.IsSectionHavingNotes(1, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetSections_ShouldReturnEmptyList_WhenNoSectionsExist()
    {
        // Arrange

        // Act
        var result = await sectionsService.GetSections(CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSections_ShouldReturnNotEmptyList_WhenSectionsExist()
    {
        // Arrange
        await sectionsService.CreateSection("Test 1", "Test 1", "#000000", CancellationToken.None);
        await sectionsService.CreateSection("Test 2", "Test 2", "#000000", CancellationToken.None);
        await sectionsService.CreateSection("Test 3", "Test 3", "#000000", CancellationToken.None);

        // Act
        var result = await sectionsService.GetSections(CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);

        // Check if the sections are ordered by Position in ascending order
        var previousPosition = 0u;
        foreach (var section in result)
        {
            Assert.True(section.Position > previousPosition);
            previousPosition = section.Position;
        }
    }

    [Fact]
    public async Task CreateSection_ShouldAddSection()
    {
        // Arrange
        var sectionName = "NewSection";
        var displayName = "New Section Display Name";
        var color = "#FFFFFF";

        // Act
        var sectionId = await sectionsService.CreateSection(sectionName, displayName, color, CancellationToken.None);
        var section = await DbContext.Sections.FindAsync(sectionId);

        // Assert
        Assert.NotNull(section);
        Assert.Equal(sectionName, section.Name);
        Assert.Equal(displayName, section.DisplayName);
        Assert.Equal(color, section.Color);
    }

    [Fact]
    public async Task DeleteSection_ShouldRemoveSection()
    {
        // Arrange
        var sectionId = await sectionsService.CreateSection("Test", "Test", "#000000", CancellationToken.None);

        // Act
        await sectionsService.DeleteSection(sectionId, CancellationToken.None);
        var deletedSection = await DbContext.Sections.FindAsync(sectionId);

        // Assert
        Assert.Null(deletedSection);
    }

    [Fact]
    public async Task UpdateSection_ShouldModifySection()
    {
        // Arrange
        var section = new SectionEntity { 
            Name = "SectionToUpdate", 
            DisplayName = "Old Display Name", 
            Color = "#000000",
            Position = 0
        };
        DbContext.Sections.Add(section);
        await DbContext.SaveChangesAsync();

        var newDisplayName = "New Display Name";
        var newColor = "#FFFFFF";

        // Act
        await sectionsService.UpdateSection(section.Id, newDisplayName, newColor, CancellationToken.None);
        var updatedSection = await DbContext.Sections.FindAsync(section.Id);

        // Assert
        Assert.NotNull(updatedSection);
        Assert.Equal(newDisplayName, updatedSection.DisplayName);
        Assert.Equal(newColor, updatedSection.Color);
    }

    [Fact]
    public async Task ReorderSections_ShouldUpdatePositions()
    {
        // Arrange
        var section1 = new SectionEntity { 
            Name = "Section1",
            DisplayName = "Section 1",
            Color = "#000000",
            Position = 0 
        };
        var section2 = new SectionEntity { 
            Name = "Section2",
            DisplayName = "Section 2",
            Color = "#000000",
            Position = 1 
        };
        DbContext.Sections.AddRange(section1, section2);
        await DbContext.SaveChangesAsync();

        var newOrder = new List<long> { section2.Id, section1.Id };

        // Act
        var result = await sectionsService.ReorderSections(newOrder, CancellationToken.None);
        var updatedSection1 = await DbContext.Sections.FindAsync(section1.Id);
        var updatedSection2 = await DbContext.Sections.FindAsync(section2.Id);

        // Assert
        Assert.True(result);
        Assert.NotNull(updatedSection1);
        Assert.NotNull(updatedSection2);
        Assert.Equal(0u, updatedSection2.Position);
        Assert.Equal(1u, updatedSection1.Position);
    }
}