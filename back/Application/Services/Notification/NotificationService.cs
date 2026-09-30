using Application.Interfaces;
using Application.Mapper;

namespace Application.Services.Notification;

public class NotificationService(IAppDbContext appDbContext, INotifier notifier, NotificationMapper mapper) : INotificationService
{
    public void FlushPendingAsync(Guid userId)
    {
        var notifications = appDbContext.Notifications
            .Where(n => !n.ReadAt.HasValue)
            .Where(n => n.RecipientId == userId)
            .ToList();

        foreach (var notification in notifications)
        {
            notifier.SendNotificationAsync(mapper.ToDto(notification));
        }
    }
}