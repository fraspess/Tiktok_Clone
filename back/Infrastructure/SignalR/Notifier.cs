using Application.Dtos.Notification;
using Application.Interfaces;
using Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR;

public class Notifier(IHubContext<NotificationHubClient, INotificationHubClient> hubContext) : INotifier
{
    public async Task SendNotificationAsync(NotificationDto notification)
    {
        await hubContext.Clients.User(notification.RecipientId.ToString()).ReceiveNotification(notification);
    }
}
