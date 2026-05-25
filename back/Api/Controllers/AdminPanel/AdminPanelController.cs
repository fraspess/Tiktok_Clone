using Application;
using Application.Constants;
using Application.Dtos.User;
using Application.Features.AdminPanel.BanUser;
using Application.Features.AdminPanel.GetUserById;
using Application.Features.AdminPanel.GetUsers;
using Application.Features.AdminPanel.UnbanUser;
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
        var users = await _mediator.Send(new AdminPanelGetUsersCommand(new PaginationSettings{PageNumber = pageNumber, PageSize = pageSize}));
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
     
}