namespace Application.Dtos.User;

public class UserDto
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string Description { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsOwnProfile { get; set; }
    public object Avatar { get; set; } = string.Empty;
    public bool IsFollowing { get; set; }
}