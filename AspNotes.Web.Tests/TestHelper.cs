﻿using AspNotes.Core.Book.Models;
using AspNotes.Core.Common;
using AspNotes.Core.Common.Models;
using AspNotes.Core.Note.Models;
using AspNotes.Core.NoteTag.Models;
using AspNotes.Core.Section.Models;
using AspNotes.Core.Tag.Models;
using AspNotes.Core.User.Models;
using AspNotes.Web.Helpers;
using AspNotes.Web.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Security.Claims;

namespace AspNotes.Web.Tests.Helpers;

internal static class TestHelper
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

    internal static IConfigurationRoot GetConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        if (configurationBuilder == null)
        {
            throw new InvalidOperationException("ConfigurationBuilder not found");
        }

        var configuration = configurationBuilder.Build();

        if (configuration == null)
        {
            throw new InvalidOperationException("Configuration not found");
        }

        return configuration;
    }

    internal static JwtSettings GetJwtSettings()
    {
        var configuration = GetConfiguration();

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

        if (jwtSettings == null)
        {
            throw new InvalidOperationException("JwtSettings not found in appsettings.json");
        }

        return jwtSettings;
    }

    internal static AllNotesSection GetAllNotesSection()
    {
        var configuration = GetConfiguration();

        var allNotesSection = configuration.GetSection("AllNotesSection").Get<AllNotesSection>();

        if (allNotesSection == null)
        {
            throw new InvalidOperationException("AllNotesSection not found in appsettings.json");
        }

        return allNotesSection;
    }

    internal static string GetTestUserToken()
    {
        var jwtSettings = GetJwtSettings();
        (string defaultUserEmail, string defaultUserPassword) = GetDefaultUserCredentials();

        var user = new UserEntity
        {
            Id = 1,
            Email = defaultUserEmail,
        };

        return JwtHelper.GenerateJwtToken(user, jwtSettings);
    }

    internal static (string email, string password) GetDefaultUserCredentials()
    {
        var configuration = GetConfiguration();

        var defaultUserEmail = configuration["DefaultUser:Email"];
        var defaultUserPassword = configuration["DefaultUser:Password"];

        if (string.IsNullOrEmpty(defaultUserEmail) || string.IsNullOrEmpty(defaultUserPassword))
        {
            throw new InvalidOperationException("DefaultUser:Email or DefaultUser:Password not found in appsettings.json");
        }

        return (defaultUserEmail, defaultUserPassword);
    }

    internal static ClaimsPrincipal GetTestUserClaimsPrincipal()
    {
        (string defaultUserEmail, string defaultUserPassword) = GetDefaultUserCredentials();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, defaultUserEmail),
            new(ClaimTypes.Name, defaultUserEmail),
            new(ClaimTypes.NameIdentifier, "1")
        };

        var identity = new ClaimsIdentity(claims);

        return new ClaimsPrincipal(identity);
    }

    internal static void ClearDatabase(NotesDbContext context)
    {
        var tables = new List<DbTable>
        {
            new NotesTable(),
            new TagsTable(),
            new BooksTable(),
            new NotesTagsTable(),
            new SectionsTable(),
        };

        var sqlDelete = string.Join(";", tables.Select(table => $"DELETE FROM {table.TableName}"));
        var sqlResetSeq = string.Join(";", tables.Select(table => $"DELETE FROM sqlite_sequence WHERE name='{table.TableName}'"));

        context.Database.ExecuteSqlRaw(sqlDelete);
        context.Database.ExecuteSqlRaw(sqlResetSeq);
    }

    internal static void ClearCache(IMemoryCache cache)
    {
        cache.Remove(CacheKeys.Sections);
    }
}
