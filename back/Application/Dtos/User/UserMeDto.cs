namespace Application.Dtos.User;

public class UserMeDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }

    public AvatarDto Avatar { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}