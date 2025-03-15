using AspNotes.Entities;
using AspNotes.Helpers;
using AspNotes.Models;
using Microsoft.Extensions.Configuration;

namespace AspNotes.Tests;
internal static class TestHelper
{
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

    internal static string GetTestUserToken()
    {
        var jwtSettings = GetJwtSettings();
        (string defaultUserEmail, _) = GetDefaultUserCredentials();

        var user = new UserEntity
        {
            Id = 1,
            Email = defaultUserEmail,
        };

        return JwtHelper.GenerateJwtToken(user, jwtSettings);
    }

    public static string GenerateTestToken(UserEntity user)
    {
        var jwtSettings = GetJwtSettings();

        return JwtHelper.GenerateJwtToken(user, jwtSettings);
    }
}
