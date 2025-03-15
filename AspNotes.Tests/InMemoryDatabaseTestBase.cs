using AspNotes.Common;
using Microsoft.EntityFrameworkCore;

namespace AspNotes.Tests;

/// <summary>
/// Provides a base class for tests that require an in-memory database.
/// </summary>
public abstract class InMemoryDatabaseTestBase : IDisposable
{
    /// <summary>
    /// Gets the in-memory database context.
    /// </summary>
    protected readonly AppDbContext DbContext;

    private readonly DbContextOptions<AppDbContext> Options;
    private static readonly EntityMaterializedInterceptor EntityMaterializedInterceptorInstance = new();
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryDatabaseTestBase"/> class.
    /// Sets up the in-memory database context.
    /// </summary>
    protected InMemoryDatabaseTestBase()
    {
        Options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        DbContext = new AppDbContext(Options, EntityMaterializedInterceptorInstance);
        DbContext.Database.OpenConnection();
        DbContext.Database.Migrate();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            DbContext.Database.CloseConnection();
            DbContext.Dispose();
        }

        disposed = true;
    }
}
