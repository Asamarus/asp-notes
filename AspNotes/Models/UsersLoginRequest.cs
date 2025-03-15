using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request to log in a user.
/// </summary>
public sealed class UsersLoginRequest
{
    /// <summary>
    /// Gets or sets the email of the user.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}
