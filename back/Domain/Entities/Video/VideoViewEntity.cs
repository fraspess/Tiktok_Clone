using Domain.Entities.Identity;

namespace Domain.Entities.Video;

public class VideoViewEntity : AuditableEntity
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    
    public Guid VideoId { get; set; }
    public VideoEntity? Video { get; set; }
    
    public DateTime ViewedAt { get; set; }
}