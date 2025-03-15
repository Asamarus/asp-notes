using System.Text.Json.Serialization;

namespace AspNotes.Models;

/// <summary>
/// Represents the response after a successful login.
/// </summary>
public sealed class UsersLoginResponse
{
    /// <summary>
    /// Gets or sets the ID of the user.
    /// </summary>
    public required long Id { get; set; }

    /// <summary>
    /// Gets or sets the email of the user.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the access token for the user.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccessToken { get; set; }
}