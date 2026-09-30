using Application.Dtos.Notification;
using Application.Extensions;
using Application.Features.User.Register;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.Notification;

public class GetNotificationCommandHandler(IAppDbContext appDbContext, NotificationMapper mapper, ICurrentUser user) : IRequestHandler<GetNotificationsCommand, PagedResult<NotificationDto>>
{
    public async Task<PagedResult<NotificationDto>> Handle(GetNotificationsCommand request, CancellationToken cancellationToken)
    {
        var notifications = await appDbContext.Notifications
            .Where(n => n.RecipientId == user.Id)
            .Where(n => !n.IsRead)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        var mapped = notifications.MapItems(mapper.ToDto);
        return mapped;
    }
}