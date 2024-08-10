using AspNotes.Core.Common;
using AspNotes.Core.Note.Models;
using AspNotes.Core.Section;
using AspNotes.Core.Section.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AspNotes.Core.Tests.Section;

public class SectionsServiceTests : DatabaseTestBase
{
    private readonly MemoryCache _memoryCache;
    private readonly AppCache _appCache;
    private readonly SectionsService _sectionsService;

    public SectionsServiceTests()
    {
        var options = new MemoryCacheOptions();
        _memoryCache = new MemoryCache(options);
        _appCache = new AppCache(_memoryCache);
        _sectionsService = new SectionsService(DbFixture.DbContext, _appCache);
    }

    public override void Dispose()
    {
        _memoryCache.Dispose();

        base.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task IsSectionNameUnique_ShouldReturnFalse_WhenNameExists()
    {
        // Arrange
        await _sectionsService.CreateSection("Test", "Test", "#000000");

        // Act
        var result = await _sectionsService.IsSectionNameUnique("Test");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsSectionNameUnique_ShouldReturnTrue_WhenNameDoesNotExist()
    {
        // Arrange
        await _sectionsService.CreateSection("Test", "Test", "#000000");

        // Act
        var result = await _sectionsService.IsSectionNameUnique("NonExistent");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSectionIdPresent_ShouldReturnFalse_WhenIdDoesNotExist()
    {
        // Arrange

        // Act
        var result = await _sectionsService.IsSectionIdPresent(9999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsSectionIdPresent_ShouldReturnTrue_WhenIdExists()
    {
        // Arrange
        var sectionId = await _sectionsService.CreateSection("Test", "Test", "#000000");

        // Act
        var result = await _sectionsService.IsSectionIdPresent(sectionId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSectionNameValid_ShouldReturnFalse_WhenNameIsInvalid()
    {
        // Arrange

        // Act
        var result = await _sectionsService.IsSectionNameValid("Test");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsSectionNameValid_ShouldReturnTrue_WhenNameIsValid()
    {
        // Arrange
        await _sectionsService.CreateSection("Test", "Test", "#000000");

        // Act
        var result = await _sectionsService.IsSectionNameValid("Test");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSectionHavingNotes_ShouldReturnTrue_WhenSectionIsHavingNotes()
    {
        // Arrange
        await _sectionsService.CreateSection("Test", "Test", "#000000");

        DbFixture.DbContext.Notes.Add(new NoteEntity
        {
            Section = "Test",
            Title = "Test",
            Content = "Test",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });

        await DbFixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _sectionsService.IsSectionHavingNotes("Test");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsSectionHavingNotes_ShouldReturnFalse_WhenSectionIsNotHavingNotes()
    {
        // Arrange

        // Act
        var result = await _sectionsService.IsSectionNameValid("Test");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetSections_ShouldReturnEmptyList_WhenNoSectionsExist()
    {
        // Arrange

        // Act
        var result = await _sectionsService.GetSections();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSections_ShouldReturnNotEmptyList_WhenSectionsExist()
    {
        // Arrange
        await _sectionsService.CreateSection("Test 1", "Test 1", "#000000");
        await _sectionsService.CreateSection("Test 2", "Test 2", "#000000");
        await _sectionsService.CreateSection("Test 3", "Test 3", "#000000");

        // Act
        var result = await _sectionsService.GetSections();

        // Assert
        Assert.NotEmpty(result);

        // Check if the sections are ordered by Position in ascending order
        uint previousPosition = 0;
        foreach (var section in result)
        {
            Assert.True(section.Position > previousPosition);
            previousPosition = section.Position;
        }
    }

    [Fact]
    public async Task CreateSection_ShouldCreateNewSection_WhenSectionDoesNotExist()
    {
        // Arrange

        // Act
        var result = await _sectionsService.CreateSection("Test", "Test", "#000000");

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public async Task UpdateSection_ShouldUpdateExistingSection_WhenSectionExists()
    {
        // Arrange
        var sectionId = await _sectionsService.CreateSection("Test", "Test", "#000000");

        // Act
        var updated = await _sectionsService.UpdateSection(1, "UpdatedTest", "#FFFFFF");

        // Assert
        Assert.True(updated);

        var section = await DbFixture.DbContext.Sections.FindAsync(sectionId);
        Assert.NotNull(section);
        Assert.Equal("UpdatedTest", section.DisplayName);
        Assert.Equal("#FFFFFF", section.Color);
    }

    [Fact]
    public async Task DeleteSection_ShouldDeleteExistingSection_WhenSectionExists()
    {
        // Arrange
        await _sectionsService.CreateSection("Test 1", "Test 1", "#000000");

        var sectionId = await _sectionsService.CreateSection("Test 2", "Test 2", "#000000");

        await _sectionsService.CreateSection("Test 3", "Test 3", "#000000");

        // Act
        var deleted = await _sectionsService.DeleteSection(sectionId);

        // Assert
        Assert.True(deleted);

        var section = await DbFixture.DbContext.Sections.FindAsync(sectionId);
        Assert.Null(section);

        var sections = await _sectionsService.GetSections();

        // Check if the sections are ordered by Position in ascending order
        uint previousPosition = 0;
        foreach (var s in sections)
        {
            Assert.True(s.Position > previousPosition);
            previousPosition = s.Position;
        }
    }

    [Fact]
    public async Task ReorderSections_ShouldReorderExistingSections_WhenSectionIdsAreValid()
    {
        // Arrange
        var sections = new List<SectionDto>
        {
            new() {
                Name = "Test1",
                DisplayName = "Test1",
                Color = "#000000",
            },
            new() {
                Name = "Test2",
                DisplayName = "Test2",
                Color = "#000000",
            },
            new() {
                Name = "Test3",
                DisplayName = "Test3",
                Color = "#000000",
            }
        };

        var sectionIds = new List<long>();
        foreach (var section in sections)
        {
            sectionIds.Add(await _sectionsService.CreateSection(section.Name, section.DisplayName, section.Color));
        }

        sectionIds.Reverse();

        // Act
        var result = await _sectionsService.ReorderSections(sectionIds);

        // Assert
        Assert.True(result);

        var orderedSections = await _sectionsService.GetSections();
        Assert.Equal(sectionIds, orderedSections.Select(s => s.Id).ToList());
    }
}
