namespace AspNotes.Web.Models;

public class JwtSettings
{
    public string Secret { get; set; } = null!;
    public string ValidIssuer { get; set; } = null!;
    public string ValidAudience { get; set; } = null!;
    public string CookieName { get; set; } = null!;
    public int AccessTokenExpirationMinutes { get; set; }

    public JwtSettings()
    {
    }
}
