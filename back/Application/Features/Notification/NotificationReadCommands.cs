using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notification;

public record GetUnreadNotificationCountQuery : IRequest<int>;
public record MarkNotificationsReadCommand(Guid? Id = null) : IRequest<Unit>;

public class GetUnreadNotificationCountHandler(IAppDbContext db, ICurrentUser user)
    : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    public Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken ct) =>
        db.Notifications.CountAsync(n => n.RecipientId == user.Id && n.ReadAt == null, ct);
}

public class MarkNotificationsReadHandler(IAppDbContext db, ICurrentUser user)
    : IRequestHandler<MarkNotificationsReadCommand, Unit>
{
    public async Task<Unit> Handle(MarkNotificationsReadCommand request, CancellationToken ct)
    {
        var query = db.Notifications.Where(n => n.RecipientId == user.Id && n.ReadAt == null);
        if (request.Id.HasValue) query = query.Where(n => n.Id == request.Id);
        await query.ExecuteUpdateAsync(s => s.SetProperty(n => n.ReadAt, DateTime.UtcNow), ct);
        return Unit.Value;
    }
}
