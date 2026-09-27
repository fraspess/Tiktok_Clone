using Domain.Entities.Identity;

namespace Domain.Entities.Video;

public class VideoLikeEntity : AuditableEntity
{
    public required Guid UserId { get; set; }
    public Guid VideoId { get; set; }

    public UserEntity? User { get; set; }

    public VideoEntity? Video { get; set; }
}