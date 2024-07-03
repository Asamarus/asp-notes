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
using Microsoft.Extensions.Logging;

namespace AspNotes.Web.Tests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<NotesDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            services.AddDbContext<NotesDbContext>(options =>
            {
                options.UseSqlite(_connection, options => options.MigrationsAssembly("AspNotes.Web"));
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();

            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<NotesDbContext>();
            var loggerFactory = scopedServices.GetRequiredService<ILoggerFactory>();
            var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

            try
            {                
                db.Database.Migrate();

                // Seed the database with test data.
                SeedData(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or initializing the database.");
            }
        });
    }

    private void SeedData(NotesDbContext context)
    {
        //var salt = UsersHelper.GenerateSalt();

        //(var userEmail, var userPassword) = TestHelper.GetDefaultUserCredentials();

        //context.Users.Add(new UserEntity { Email = userEmail, PasswordHash = UsersHelper.HashPassword(userPassword, salt), Salt = salt });
        //context.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
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
