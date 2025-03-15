using AspNotes.Common;
using AspNotes.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AspNotes.Tests.Common;

public class AppDbContextTests
{
    private readonly DbContextOptions<AppDbContext> options;
    private readonly Mock<EntityMaterializedInterceptor> entityMaterializedInterceptorMock;

    public AppDbContextTests()
    {
        options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        entityMaterializedInterceptorMock = new Mock<EntityMaterializedInterceptor>();
    }

    [Fact]
    public void SaveChanges_ShouldUpdateTimestamps()
    {
        // Arrange
        using var context = new AppDbContext(options, entityMaterializedInterceptorMock.Object);
        var entity = new NoteEntity { Id = 1, Title = "Test Note", Section = "Test" };
        context.Notes.Add(entity);

        // Act
        context.SaveChanges();

        // Assert
        Assert.NotEqual(default, entity.CreatedAt);
        Assert.NotEqual(default, entity.UpdatedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldUpdateTimestamps()
    {
        // Arrange
        using var context = new AppDbContext(options, entityMaterializedInterceptorMock.Object);
        var entity = new NoteEntity { Id = 2, Title = "Test Note", Section = "Test" };
        context.Notes.Add(entity);

        // Act
        await context.SaveChangesAsync();

        // Assert
        Assert.NotEqual(default, entity.CreatedAt);
        Assert.NotEqual(default, entity.UpdatedAt);
    }
}
