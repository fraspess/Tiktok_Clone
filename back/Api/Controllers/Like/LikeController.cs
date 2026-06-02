using Application;
using Application.Extensions;
using Application.Features.LIke.ToogleLike;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Like
{
    [Route("api/likes")]
    [ApiController]
    public class LikeController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToogleLike(Guid videoId)
        {
            await _mediator.Send(new ToogleLikeCommand(videoId));
            return Ok(ApiResponse<object>.Success(null!, null));
        }
    }
}