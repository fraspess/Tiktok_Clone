using Application.Dtos.Notification;
using Application.Dtos.User;
using Domain;

namespace Application.Interfaces;

public interface INotifier
{
    Task SendNotificationAsync(NotificationDto dto);
}