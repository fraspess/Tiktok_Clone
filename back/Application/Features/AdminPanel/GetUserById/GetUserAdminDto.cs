using Domain;

namespace Application.Features.AdminPanel.GetUserById;

public class GetUserAdminDto
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string Description { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public object Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsBanned { get; set; }
    public UserReportReasons? BanReason { get; set; }
    public Guid? BannedBy { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get;set; }
}