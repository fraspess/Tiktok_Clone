using Application.Dtos.User;
using Domain.Entities.Identity;

namespace Application.Extensions;

public static class UserQueryExtensions
{
    public static IQueryable<UserProjectionDto> ToProjectionDto(this IQueryable<UserEntity> users, Guid? currentUserId)
    {
        return users.Select(u => new UserProjectionDto()
        {
            Id = u.Id,  
            Username = u.UserName,
            Description = u.Description,
            Email =  u.Email,
            FollowersCount = u.Followers.Count,
            FollowingCount = u.Following.Count,
            IsFollowing = currentUserId.HasValue && u.Followers.Any(f => f.FollowerId == currentUserId),
            IsOwnProfile = currentUserId.HasValue && u.Id == currentUserId
        });
    }
}