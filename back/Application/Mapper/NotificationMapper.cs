using Application.Dtos.Notification;
using Domain.Entities.Notification;
using Riok.Mapperly.Abstractions;

namespace Application.Mapper;

[Mapper]
public partial class NotificationMapper
{
    public partial NotificationDto ToDto(NotificationEntity source);
}