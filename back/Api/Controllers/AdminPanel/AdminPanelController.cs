using Application;
using Application.Constants;
using Application.Dtos.User;
using Application.Features.AdminPanel.BanUser;
using Application.Features.AdminPanel.BanVideo;
using Application.Features.AdminPanel.GetReports;
using Application.Features.AdminPanel.GetUserById;
using Application.Features.AdminPanel.GetUsers;
using Application.Features.AdminPanel.GetUserVideos;
using Application.Features.AdminPanel.GetVideos;
using Application.Features.AdminPanel.UnbanUser;
using Application.Features.AdminPanel.UnbanVideo;
using Application.Features.Comment.Delete;
using Application.Pagination;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.AdminPanel;

[Route("api/admin-panel")]
[ApiController]
[Authorize(Roles = RoleNames.ADMIN_ROLE + "," + RoleNames.SUPER_ADMIN_ROLE)]
public class AdminPanelController(IMediator _mediator) : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
    {
        var users = await _mediator.Send(new AdminPanelGetUsersCommand(new PaginationSettings
            { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<object>.Success(users));
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdCommand(id));
        return Ok(ApiResponse<object>.Success(user));
    }

    [HttpPost("users/ban")]
    public async Task<IActionResult> BanUser(Guid id, UserReportReasons reason)
    {
        await _mediator.Send(new BanUserCommand(id, reason));
        return Ok(ApiResponse<object>.Success(null!));
    }

    [HttpPost("users/unban")]
    public async Task<IActionResult> UnBanUser(Guid id)
    {
        await _mediator.Send(new UnbanUserCommand(id));
        return Ok(ApiResponse<object>.Success(null!));
    }


    [HttpGet("videos")]
    public async Task<IActionResult> GetVideos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
    {
        var videos = await _mediator.Send(new AdminPanelGetVideosCommand(new PaginationSettings
            { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<object>.Success(videos));
    }

    [HttpPost("videos/ban")]
    public async Task<IActionResult> BanVideo(Guid id, VideoReportReasons reason)
    {
        await _mediator.Send(new BanVideoCommand(id, reason));
        return Ok(ApiResponse<object>.Success(null!));
    }

    [HttpPost("videos/unban")]
    public async Task<IActionResult> UnbanVideo(Guid id)
    {
        await _mediator.Send(new UnbanVideoCommand(id));
        return Ok(ApiResponse<object>.Success(null!));
    }

    [HttpGet("users/video")]
    public async Task<IActionResult> GetUserVideos([FromQuery] Guid id, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5)
    {
        var videos = await _mediator.Send(new AdminPanelGetUserVideosCommand(id,
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));

        return Ok(ApiResponse<object>.Success(videos));
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetReports([FromQuery] ContentTypes reportType, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5)
    {
        var reports = await _mediator.Send(new AdminPanelGetReportsCommand(reportType,
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));

        return Ok(ApiResponse<object>.Success(reports));
    }

    [HttpDelete("comments/{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        await _mediator.Send(new DeleteCommentCommand(id));
        return Ok(ApiResponse<object>.Success(null!));
    }
}