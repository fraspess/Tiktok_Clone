using Domain.Entities.Identity;

namespace Domain.Entities.Video;

public class VideoRepostEntity : AuditableEntity
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    
    public Guid VideoId { get; set; }
    public VideoEntity? Video { get; set; }
    
}