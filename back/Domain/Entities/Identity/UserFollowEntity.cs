namespace Domain.Entities.Identity;

public class UserFollowEntity
{
    public Guid FollowerId { get; set; }
    public UserEntity Follower { get; set; } = null!;

    public Guid FollowingId { get; set; }
    public UserEntity Following { get; set; } = null!;

    public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
}