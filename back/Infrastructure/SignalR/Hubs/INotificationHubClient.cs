using Application.Dtos.Notification;

namespace Infrastructure.SignalR.Hubs;

public interface INotificationHubClient
{
    Task ReceiveNotification(NotificationDto notification);
}
