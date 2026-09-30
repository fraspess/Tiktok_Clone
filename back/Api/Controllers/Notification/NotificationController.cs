
using Application;

using Application.Features.Notification;
using Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Notification;

[Route("api/notifications")]
[ApiController]
[Authorize]
public class NotificationController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetNotificationsCommand(new PaginationSettings
            {PageNumber = pageNumber, PageSize = pageSize}), ct);
        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct) =>
        Ok(ApiResponse<int>.Success(await _mediator.Send(new GetUnreadNotificationCountQuery(), ct)));

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new MarkNotificationsReadCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        await _mediator.Send(new MarkNotificationsReadCommand(), ct);
        return NoContent();
    }
}
