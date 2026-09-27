using Domain.Entities.Identity;

namespace Domain.Entities.Comment;

public class CommentLikeEntity
{
    public Guid CommentId { get; set; }

    public Guid UserId { get; set; }

    public UserEntity? User { get; set; }

    public CommentEntity? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}