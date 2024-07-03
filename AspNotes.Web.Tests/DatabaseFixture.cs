using AspNotes.Core.Common;
using AspNotes.Web.Tests.Helpers;
using Microsoft.Data.Sqlite;
using SqlKata.Execution;

namespace AspNotes.Web.Tests;

public class DatabaseFixture : IDisposable
{
    public NotesDbContext DbContext { get; private set; }
    public QueryFactory QueryFactory { get; private set; }
    private SqliteConnection Connection { get; set; }

    public DatabaseFixture()
    {
        Connection = TestHelper.CreateInMemoryDatabase();
        DbContext = TestHelper.GetNotesDbContext(Connection);
        QueryFactory = TestHelper.GetQueryFactory(Connection);
    }

    public void Dispose()
    {
        DbContext.Dispose();
        QueryFactory.Dispose();
        Connection.Close();
        GC.SuppressFinalize(this);
    }
}

public class DatabaseTestBase : IDisposable
{
    protected readonly DatabaseFixture DbFixture;

    public DatabaseTestBase()
    {
        DbFixture = new DatabaseFixture();
    }

    public void Dispose()
    {
        DbFixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
