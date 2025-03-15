using AspNotes.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AspNotes.Common;

/// <summary>
/// Intercepts the materialization of entities.
/// </summary>
public class EntityMaterializedInterceptor : IMaterializationInterceptor
{
    /// <summary>
    /// Called when an entity instance is initialized during materialization.
    /// </summary>
    /// <param name="materializationData">The data related to the materialization process.</param>
    /// <param name="entity">The entity instance being initialized.</param>
    /// <returns>The initialized entity instance.</returns>
    public object InitializedInstance(MaterializationInterceptionData materializationData, object entity)
    {
        if (entity is NoteEntity noteEntity)
        {
            noteEntity.SetTags(noteEntity.Tags);
        }

        return entity;
    }
}
