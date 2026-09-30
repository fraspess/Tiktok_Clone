namespace Domain.Entities.Notification;

public class NotificationEntity : AuditableEntity
{
    public Guid RecipientId { get; set; }
    
    public Guid? ActorId { get; set; }
    
    public NotificationType Type { get; set; }
    
    public Guid? ResourceId { get; set; }
    
    public Guid? ConversationId { get; set; }
    
    public DateTime? ReadAt { get; set; }
    public bool IsRead => ReadAt.HasValue;
    
    
}