using AspNotes.Core.Common;
using Microsoft.Data.Sqlite;
using SqlKata;
using SqlKata.Execution;

namespace AspNotes.Core.Tests;

public class DatabaseFixture : IDisposable
{
    public NotesDbContext DbContext { get; private set; }

    public QueryFactory Db { get; private set; }

    public List<SqlResult> SqlResults = [];

    private SqliteConnection Connection { get; set; }

    public DatabaseFixture()
    {
        Connection = TestHelper.CreateInMemoryDatabase();
        DbContext = TestHelper.GetNotesDbContext(Connection);
        Db = TestHelper.GetQueryFactory(Connection);

        Db.Logger = compiled =>
        {
            SqlResults.Add(compiled);
        };
    }

    public void Dispose()
    {
        DbContext.Dispose();
        Db.Dispose();
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

    public virtual void Dispose()
    {
        DbFixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
