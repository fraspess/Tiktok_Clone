using Application.Dtos.Notification;
using Application.Dtos.User;
using Application.Interfaces;
using Domain;
using Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR;

public class Notifier(IHubContext<NotificationHubClient> hubContext) : INotifier
{
    public async Task SendNotificationAsync(NotificationDto notification)
    {
        await hubContext.Clients.User(notification.RecipientId.ToString()).SendAsync("ReceiveNotification", notification);
    }
}