namespace AspNotes.Models;

/// <summary>
/// Represents the response containing the current user's information.
/// </summary>
public sealed class UsersCurrentUserResponse
{
    /// <summary>
    /// Gets or sets the ID of the user.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the email of the user.
    /// </summary>
    public required string Email { get; set; }
}
