using Domain.Entities.Identity;

namespace Application.Dtos.User;

public class UserProjectionDto
{
    public Guid Id { get; set; }
    public string? Username { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsOwnProfile { get; set; }
}