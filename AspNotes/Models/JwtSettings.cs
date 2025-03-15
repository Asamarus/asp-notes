namespace AspNotes.Models;

/// <summary>
/// Represents the settings required for JWT authentication.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>
    /// Gets or sets the secret key used for signing the JWT.
    /// </summary>
    public required string Secret { get; set; }

    /// <summary>
    /// Gets or sets the valid issuer for the JWT.
    /// </summary>
    public required string ValidIssuer { get; set; }

    /// <summary>
    /// Gets or sets the valid audience for the JWT.
    /// </summary>
    public required string ValidAudience { get; set; }

    /// <summary>
    /// Gets or sets the name of the cookie used to store the JWT.
    /// </summary>
    public required string CookieName { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for the access token in minutes.
    /// </summary>
    public required int AccessTokenExpirationMinutes { get; set; }
}