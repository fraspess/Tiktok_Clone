using System.Security.Claims;
using Application.Extensions;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? Id
    {
        get
        {
            var id = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == null) return null;
            return Guid.Parse(id);
        }
    }

    public string? Email => httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}