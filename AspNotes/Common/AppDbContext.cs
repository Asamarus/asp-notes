using AspNotes.Entities;
using AspNotes.Helpers;
using AspNotes.Interfaces;
using AspNotes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AspNotes.Common;

/// <summary>
/// Represents the application database context.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AppDbContext"/> class.
/// </remarks>
/// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
/// <param name="entityMaterializedInterceptor">The entity materialized interceptor.</param>
public class AppDbContext(DbContextOptions<AppDbContext> options, EntityMaterializedInterceptor entityMaterializedInterceptor)
    : DbContext(options), IAppDbContext
{
    /// <summary>
    /// Gets or sets the DbSet for <see cref="TagEntity"/>.
    /// </summary>
    public DbSet<TagEntity> Tags { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for <see cref="SectionEntity"/>.
    /// </summary>
    public DbSet<SectionEntity> Sections { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for <see cref="NoteEntity"/>.
    /// </summary>
    public DbSet<NoteEntity> Notes { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for <see cref="NoteFtsEntity"/>.
    /// </summary>
    public DbSet<NoteFtsEntity> NotesFts { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for <see cref="NoteTagEntity"/>.
    /// </summary>
    public DbSet<NoteTagEntity> NotesTags { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for <see cref="BookEntity"/>.
    /// </summary>
    public DbSet<BookEntity> Books { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for <see cref="UserEntity"/>.
    /// </summary>
    public DbSet<UserEntity> Users { get; set; } = null!;

    /// <summary>
    /// Overrides the SaveChanges method to update the UpdatedAt property.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Overrides the SaveChangesAsync method to update the UpdatedAt property.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Configures the database context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<NoteEntity>()
           .Property(e => e.Sources)
        .HasConversion(JsonHelper.GetJsonConverter<List<NotesSource>>())
           .Metadata.SetValueComparer(DataHelper.GetComparer<NotesSource>());

        modelBuilder.Entity<NoteEntity>()
            .HasOne(n => n.NoteFts)
            .WithOne(nf => nf.Note)
            .HasForeignKey<NoteFtsEntity>(nf => nf.Id);

        modelBuilder.Entity<NoteFtsEntity>()
             .ToView("NotesFTS")
             .HasKey(n => n.Id);
    }

    /// <summary>
    /// Configures the database context options.
    /// </summary>
    /// <param name="optionsBuilder">The builder used to create or modify options for this context.</param>
    /// <remarks>
    /// In debug mode, this method configures the context to log database commands to the debug output
    /// and enables sensitive data logging.
    /// </remarks>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(entityMaterializedInterceptor);

#if DEBUG
        optionsBuilder
            .LogTo(
                msg => System.Diagnostics.Debug.WriteLine(msg),
                [DbLoggerCategory.Database.Command.Name],
                LogLevel.Information)
            .EnableSensitiveDataLogging();
#endif
    }

    /// <summary>
    /// Updates the UpdatedAt property for all entities that extend BaseEntity.
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.Now;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.Now;
            }
        }
    }
}
