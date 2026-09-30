using Application.Dtos.Notification;
using Application.Extensions;
using Application.Interfaces;
using Application.Pagination;
using MediatR;
using Application.Services.Notification;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notification;

public class GetNotificationCommandHandler(IAppDbContext appDbContext, INotificationService service, ICurrentUser user) : IRequestHandler<GetNotificationsCommand, PagedResult<NotificationDto>>
{
    public async Task<PagedResult<NotificationDto>> Handle(GetNotificationsCommand request, CancellationToken cancellationToken)
    {
        var notifications = await appDbContext.Notifications.AsNoTracking()
            .Where(n => n.RecipientId == user.Id)
            .OrderByDescending(n => n.CreatedAt).ThenByDescending(n => n.Id)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        return new PagedResult<NotificationDto>
        {
            Items = await service.ToDtosAsync(notifications.Items, cancellationToken),
            Metadata = notifications.Metadata
        };
    }
}
