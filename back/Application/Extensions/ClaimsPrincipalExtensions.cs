using Domain.Constants;
using System.Security.Claims;
using Domain.Exceptions;

namespace Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedException(ErrorCodes.Unauthorized, "The authentication token is invalid.");
        return Guid.Parse(userId);
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        var email = user.FindFirst(ClaimTypes.Email)?.Value
                    ?? throw new UnauthorizedException(ErrorCodes.Unauthorized, "The authentication token is invalid.");
        return email;
    }
}
