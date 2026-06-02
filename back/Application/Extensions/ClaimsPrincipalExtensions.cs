using Domain.Exceptions;
using System.Security.Claims;

namespace Application.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? throw new UnauthorizedException("Користувача не знайдено. Невалідний токен");
            return Guid.Parse(userId);
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            var email =  user.FindFirst(ClaimTypes.Email)?.Value 
                         ?? throw new UnauthorizedException("Невалідний токен");
            return email;
        }
    }
}