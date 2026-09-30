using Application.Dtos.Notification;
using Domain;
using Domain.Entities.Notification;

namespace Application.Services.Notification;

public interface INotificationService
{
    // Stage alongside the triggering action, then publish only after SaveChanges succeeds.
    NotificationEntity? Create(Guid recipientId, Guid actorId, NotificationType type,
        Guid? resourceId = null, Guid? conversationId = null);
    Task PublishAsync(IEnumerable<NotificationEntity?> notifications, CancellationToken ct = default);
    Task<List<NotificationDto>> ToDtosAsync(IEnumerable<NotificationEntity> notifications, CancellationToken ct = default);
}
