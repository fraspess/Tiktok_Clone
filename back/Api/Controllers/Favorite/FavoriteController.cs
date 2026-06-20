using Api.RateLimiting;
using Application;
using Application.Extensions;
using Application.Features.Video.ToggleFavorite;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers.Favorite;

[Route("api/favorites")]
[ApiController]
public class FavoriteController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [RateLimit(20, 60_000)]
    public async Task<IActionResult> Favorite(Guid videoId)
    {
        await mediator.Send(new ToggleFavoriteCommand(videoId));
        return Ok(ApiResponse<object>.Success(null!, null));
    }
}