using System.Security.Claims;

namespace SponsorshipApproval.Api.Controllers;

public static class ControllerExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : throw new UnauthorizedAccessException("Authenticated user id is missing.");
    }

    public static string GetRole(this ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.Role) ?? throw new UnauthorizedAccessException("Authenticated role is missing.");
}
