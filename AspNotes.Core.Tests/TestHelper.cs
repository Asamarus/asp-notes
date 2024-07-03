using AspNotes.Core.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace AspNotes.Core.Tests;

public static class TestHelper
{
    internal static SqliteConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        return connection;
    }

    internal static NotesDbContext GetNotesDbContext(SqliteConnection? connection = null)
    {
        connection ??= CreateInMemoryDatabase();

        var options = new DbContextOptionsBuilder<NotesDbContext>()
            .UseSqlite(connection, options => options.MigrationsAssembly("AspNotes.Web"))
            .Options;

        var dbContext = new NotesDbContext(options);

        dbContext.Database.Migrate();

        return dbContext;
    }

    internal static QueryFactory GetQueryFactory(SqliteConnection connection)
    {
        var compiler = new SqliteCompiler();
        return new QueryFactory(connection, compiler);
    }
}
