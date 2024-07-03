using AspNotes.Core.User;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNotes.Web.Migrations;

/// <inheritdoc />
public partial class AddDefaultUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = configurationBuilder.Build();

        var email = configuration["DefaultUser:Email"];
        var password = configuration["DefaultUser:Password"];
        var salt = UsersHelper.GenerateSalt();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Default user email and password must be set in environment variables.");
        }

        string saltHex = BitConverter.ToString(salt).Replace("-", "");

        migrationBuilder.Sql($@"
                INSERT INTO Users (Email, PasswordHash, Salt, CreatedAt, UpdatedAt)
                VALUES ('{email}', '{UsersHelper.HashPassword(password, salt)}', X'{saltHex}', '{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fffffff}', '{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fffffff}')
            ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        var configurationBuilder = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = configurationBuilder.Build();

        var email = configuration["DefaultUser:Email"];

        migrationBuilder.Sql($@"
                DELETE FROM Users
                WHERE Email = '{email}'
            ");
    }
}
