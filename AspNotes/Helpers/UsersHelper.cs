using System.Security.Cryptography;

namespace AspNotes.Helpers;

/// <summary>
/// Provides helper methods for user-related operations.
/// </summary>
public static class UsersHelper
{
    /// <summary>
    /// Hashes the specified password using the provided salt.
    /// </summary>
    /// <param name="requestPassword">The password to hash.</param>
    /// <param name="salt">The salt to use for hashing.</param>
    /// <returns>The hashed password as a base64 string.</returns>
    public static string HashPassword(string requestPassword, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(requestPassword, salt, 100_000, HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(256 / 8);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Generates a cryptographic salt.
    /// </summary>
    /// <returns>A byte array representing the generated salt.</returns>
    public static byte[] GenerateSalt()
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        return salt;
    }
}