using AspNotes.Common;
using AspNotes.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AspNotes.Tests.Common;

public class EntityMaterializedInterceptorTests
{
    private readonly EntityMaterializedInterceptor interceptor;

    public EntityMaterializedInterceptorTests()
    {
        interceptor = new EntityMaterializedInterceptor();
    }

    [Fact]
    public void InitializedInstance_ShouldSetTags_ForNoteEntity()
    {
        // Arrange
        var noteEntity = new NoteEntity { Section = "Test", Tags = "tag1, tag2" };
        var materializationData = new MaterializationInterceptionData();

        // Act
        var result = interceptor.InitializedInstance(materializationData, noteEntity);

        // Assert
        Assert.IsType<NoteEntity>(result);
        var resultNoteEntity = result as NoteEntity;
        Assert.NotNull(resultNoteEntity);
        Assert.Equal(noteEntity.Tags, resultNoteEntity.Tags);
    }

    [Fact]
    public void InitializedInstance_ShouldReturnEntity_ForNonNoteEntity()
    {
        // Arrange
        var otherEntity = new object();
        var materializationData = new MaterializationInterceptionData();

        // Act
        var result = interceptor.InitializedInstance(materializationData, otherEntity);

        // Assert
        Assert.Equal(otherEntity, result);
    }
}
