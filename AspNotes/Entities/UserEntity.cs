using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNotes.Entities;

/// <summary>
/// Represents a user entity in the application.
/// </summary>
[Table("Users")]
[Index(nameof(Email), IsUnique = true)]
public class UserEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the hashed password of the user.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time of the user's last login.
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Gets or sets the salt used for hashing the user's password.
    /// </summary>
    public byte[] Salt { get; set; } = null!;
}