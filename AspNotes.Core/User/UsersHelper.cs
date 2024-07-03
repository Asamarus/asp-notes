using System.Security.Cryptography;

namespace AspNotes.Core.User;
public static class UsersHelper
{
    public static string HashPassword(string requestPassword, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(requestPassword, salt, 10000, HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(256 / 8);
        return Convert.ToBase64String(hash);
    }

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
