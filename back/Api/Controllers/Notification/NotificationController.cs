
using Application;

using Application.Features.Notification;
using Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Notification;

[Route("api/notifications")]
[ApiController]
public class NotificationController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetNotifications(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetNotificationsCommand(new PaginationSettings{PageNumber = pageNumber, PageSize = pageSize}));
        return Ok(ApiResponse<object>.Success(result));
    }
}