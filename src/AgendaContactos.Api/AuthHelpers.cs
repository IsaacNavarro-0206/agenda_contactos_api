using System.Security.Claims;

namespace AgendaContactos.Api;

public static class AuthHelpers
{
    public static bool TryGetCurrentUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out userId);
    }
}
