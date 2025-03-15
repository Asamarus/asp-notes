using AspNotes.Common;
using AspNotes.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace AspNotes.Tests;

public class ApiFactory : WebApplicationFactory<IAspNotesAssemblyMarker>
{
    private readonly DbConnection connection;
    private bool disposed = false;

    public ApiFactory()
    {
        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            {
                options.UseSqlite(connection);
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            db.Database.Migrate();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                connection?.Dispose();
            }

            disposed = true;
        }

        base.Dispose(disposing);
    }
}