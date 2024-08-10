using AspNotes.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data.Common;

namespace AspNotes.Web.Tests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContextOptions<NotesDbContext> registration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<NotesDbContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            // Remove existing DbConnection registration
            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbConnection));
            if (dbConnectionDescriptor != null)
                services.Remove(dbConnectionDescriptor);

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });

            // Register NotesDbContext with SQLite connection
            services.AddDbContext<NotesDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection, options => options.MigrationsAssembly("AspNotes.Web"));
            });
        });
    }
}

public class ProductionWebApplicationFactory<TStartup> : CustomWebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment(Environments.Production);
    }
}

public class TestControllerWithException : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("test")]
    public IActionResult Test()
    {
        throw new Exception("Test exception");
    }
}

public class WebApplicationFactoryWithException<TStartup> : CustomWebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.AddControllers().AddApplicationPart(typeof(TestControllerWithException).Assembly);
        });
    }
}

public class ProductionWebApplicationFactoryWithException<TStartup> : ProductionWebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.AddControllers().AddApplicationPart(typeof(TestControllerWithException).Assembly);
        });
    }
}
