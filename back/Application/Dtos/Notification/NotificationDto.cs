using Domain;
using Application.Dtos.User;

namespace Application.Dtos.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid RecipientId { get; set; }
    
    public Guid? ActorId { get; set; }
    
    public NotificationType Type { get; set; }
    
    public Guid? ResourceId { get; set; }
    
    public Guid? ConversationId { get; set; }
    
    public DateTime? ReadAt { get; set; }
    public SimpleUserDto? Actor { get; set; }
    public string? VideoShortId { get; set; }
}
