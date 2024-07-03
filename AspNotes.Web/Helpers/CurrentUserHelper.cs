using AspNotes.Web.Models.Common;
using System.Security.Claims;

namespace AspNotes.Web.Helpers;

public static class CurrentUserHelper
{
    public static CurrentUser GetCurrentUser(this ClaimsPrincipal claimsPrincipal)
    {
        var idClaim = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

        if (idClaim == null || email == null)
        {
            throw new Exception("User ID or email not found in claims.");
        }

        if (!int.TryParse(idClaim, out var id))
        {
            throw new Exception("User ID is not a valid integer.");
        }

        var currentUser = new CurrentUser
        {
            Id = id,
            Email = email
        };

        return currentUser;
    }
}
