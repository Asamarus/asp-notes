using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Note.Models;
using AspNotes.Core.Common.Models;
using AspNotes.Core.Book.Models;
using AspNotes.Core.NoteTag.Models;
using AspNotes.Core.Section.Models;
using AspNotes.Core.Tag.Models;
using AspNotes.Core.Tag;
using AspNotes.Core.User.Models;

namespace AspNotes.Core.Common;

public class NotesDbContext(DbContextOptions<NotesDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; } = null!;

    public DbSet<SectionEntity> Sections { get; set; } = null!;

    public DbSet<NoteEntity> Notes { get; set; } = null!;

    public DbSet<BookEntity> Books { get; set; } = null!;

    public DbSet<TagEntity> Tags { get; set; } = null!;

    public DbSet<NoteTagEntity> NotesTags { get; set; } = null!;

    public DbConnection GetDbConnection() => Database.GetDbConnection();

    public override int SaveChanges()
    {
        UpdateTimestamps();
        UpdateNotesSearchIndex();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        UpdateNotesSearchIndex();

        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateNotesSearchIndex()
    {
        var notes = ChangeTracker.Entries<NoteEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var note in notes)
        {
            if (note.Property(e => e.Title).IsModified || note.State == EntityState.Added)
            {
                var title = note.Entity.Title;
                note.Entity.TitleSearchIndex = string.IsNullOrEmpty(title) ? null : SearchHelper.GetSearchIndex(title);
            }

            if (note.Property(e => e.Content).IsModified || note.State == EntityState.Added)
            {
                var content = note.Entity.Content;
                note.Entity.ContentSearchIndex = string.IsNullOrEmpty(content) ? null : SearchHelper.GetSearchIndex(content);
                note.Entity.Preview = string.IsNullOrEmpty(content) ? null : SearchHelper.GetSearchIndex(content, false, 100);
            }
        }
    }

    private void UpdateTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(e => e.Entity is EntityBase && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

        foreach (var entityEntry in entities)
        {
            ((EntityBase)entityEntry.Entity).UpdatedAt = DateTime.Now;

            if (entityEntry.State == EntityState.Added)
            {
                ((EntityBase)entityEntry.Entity).CreatedAt = DateTime.Now;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NoteEntity>()
            .Property(e => e.Sources)
            .HasConversion(JsonHelper.GetJsonConverter<List<NoteSource>>())
            .Metadata.SetValueComparer(DataHelper.GetComparer<NoteSource>());

        modelBuilder.Entity<NoteEntity>()
            .Property(e => e.Tags)
            .HasConversion(TagsHelper.GetTagsConverter())
            .Metadata.SetValueComparer(DataHelper.GetComparer<string>());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder
            .LogTo(
                msg => System.Diagnostics.Debug.WriteLine(msg),
                new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information
            )
            .EnableSensitiveDataLogging();
#endif
    }
}