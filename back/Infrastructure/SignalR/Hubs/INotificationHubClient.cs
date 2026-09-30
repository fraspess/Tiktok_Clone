using Application.Dtos.User;
using Domain;

namespace Infrastructure.SignalR.Hubs;

public interface INotificationHubClient
{
    Task SendNotification(NotificationType type, SimpleUserDto user);
}