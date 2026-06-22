using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.RateLimiting;

public class RateLimitFilter(SlidingWindowRateLimiter _limiter) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var attr = context.ActionDescriptor
            .EndpointMetadata
            .OfType<RateLimitAttribute>()
            .LastOrDefault();

        if (attr is null)
        {
            await next();
            return;
        }

        var identifier = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var (allowed, remaining) = await _limiter.CheckAsync(
            identifier, attr.Limit, attr.WindowMs);

        context.HttpContext.Response.Headers["X-RateLimit-Limit"] = attr.Limit.ToString();
        context.HttpContext.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();

        if (!allowed)
        {
            context.Result = new ObjectResult(ApiResponse<object>.Error("Занадто багатів запитів!"))
            {
                StatusCode = 429
            };
            return;
        }

        await next();
    }
}