using Api.RateLimiting;
using Application;
using Application.Dtos.Comment;
using Application.Extensions;
using Application.Features.Comment.Create;
using Application.Features.Comment.Delete;
using Application.Features.Comment.Get;
using Application.Features.Comment.GetReplies;
using Application.Features.Comment.Like;
using Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Comment;

[Route("api/comments")]
[RateLimit(20, 60_000)]
[ApiController]
public class CommentController(IMediator _mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [RateLimit(10, 60_000)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
    {
        await _mediator.Send(new CreateCommentCommand(dto));
        return Ok(ApiResponse<object>.Success(null!, "Успішно створено коментар"));
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(Guid videoId, int pageNumber = 1, int pageSize = 20)
    {
        var comments = await _mediator.Send(new GetCommentsQuery(videoId,
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<PagedResult<CommentDto>>.Success(comments, null));
    }

    [HttpGet("replies")]
    public async Task<IActionResult> GetReplies(Guid commentId, int pageNumber = 1, int pageSize = 5)
    {
        var replies = await _mediator.Send(new GetRepliesQuery(commentId,
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<PagedResult<CommentDto>>.Success(replies, null));
    }

    [HttpDelete]
    [Authorize]
    [RateLimit(10, 60_000)]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        await _mediator.Send(new DeleteCommentCommand(commentId));
        return Ok(ApiResponse<object>.Success(null!, "Успішно видалено коментар"));
    }


    [HttpPost("like")]
    [Authorize]
    [RateLimit(20, 60_000)]
    public async Task<IActionResult> ToggleLikeComment([FromQuery] Guid commentId)
    {
        await _mediator.Send(new LikeCommentCommand(commentId));
        return Ok(ApiResponse<object>.Success(null!));
    }
}