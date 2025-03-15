using AspNotes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AspNotes.Interfaces;

/// <summary>
/// Represents the application database context interface.
/// </summary>
public interface IAppDbContext
{
    /// <summary>
    /// Gets or sets the DbSet for <see cref="TagEntity"/>.
    /// </summary>
    DbSet<TagEntity> Tags { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for <see cref="SectionEntity"/>.
    /// </summary>
    DbSet<SectionEntity> Sections { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for <see cref="NoteEntity"/>.
    /// </summary>
    DbSet<NoteEntity> Notes { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for <see cref="NoteFtsEntity"/>.
    /// </summary>
    DbSet<NoteFtsEntity> NotesFts { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for <see cref="NoteTagEntity"/>.
    /// </summary>
    DbSet<NoteTagEntity> NotesTags { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for <see cref="BookEntity"/>.
    /// </summary>
    DbSet<BookEntity> Books { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for <see cref="UserEntity"/>.
    /// </summary>
    DbSet<UserEntity> Users { get; set; }

    /// <summary>
    /// Gets the <see cref="DatabaseFacade"/> instance for the current database context.
    /// </summary>
    /// <value>
    /// The <see cref="DatabaseFacade"/> instance that provides access to database-related information and operations.
    /// </value>
    DatabaseFacade Database { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation.
    /// The task result contains the number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}