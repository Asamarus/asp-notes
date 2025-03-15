using System.ComponentModel.DataAnnotations;

namespace AspNotes.Models;

/// <summary>
/// Represents a request to change the user's password.
/// </summary>
public sealed class UsersChangePasswordRequest
{
    /// <summary>
    /// Gets or sets the current password of the user.
    /// </summary>
    [Required]
    [MinLength(6, ErrorMessage = "Minimum password length is 6")]
    public required string CurrentPassword { get; set; }

    /// <summary>
    /// Gets or sets the new password of the user.
    /// </summary>
    [Required]
    [MinLength(6, ErrorMessage = "Minimum password length is 6")]
    public required string NewPassword { get; set; }

    /// <summary>
    /// Gets or sets the confirmation of the new password.
    /// </summary>
    [Required]
    [MinLength(6, ErrorMessage = "Minimum password length is 6")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match")]
    public required string PasswordRepeat { get; set; }
}