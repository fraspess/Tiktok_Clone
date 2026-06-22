using Application.Dtos.User;
using Application.Features.AdminPanel.GetUserById;
using Application.Interfaces;
using Domain.Entities.Identity;
using Riok.Mapperly.Abstractions;

namespace Application.Mapper;

[Mapper]
public partial class UserMapper(IStorageService storageService, ICurrentUser currentUser)
{
    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserMeDto.Avatar), Use = nameof(AvatarUrl))]
    public partial UserMeDto ToMeDto(UserProjectionDto user);

    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserDto.Avatar), Use = nameof(AvatarUrl))]
    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserDto.IsOwnProfile), Use = nameof(IsOwnProfile))]
    public partial UserDto ToDto(UserProjectionDto user);

    [MapProperty(nameof(RegisterUserDto.Username), nameof(UserEntity.UserName))]
    public partial UserEntity ToEntity(RegisterUserDto dto);

    public partial GetUserAdminDto ToGetUserAdminDto(UserEntity source);

    [MapProperty(nameof(UserProjectionDto.Id), nameof(SimpleUserDto.Avatar), Use = nameof(AvatarUrl))]
    public partial SimpleUserDto ToSimpleDto(UserProjectionDto source);


    [UserMapping(Default = false)]
    private bool IsOwnProfile(Guid userId)
    {
        return currentUser.Id == userId;
    }

    [UserMapping(Default = false)]
    private bool IsFollowing(ICollection<UserFollowEntity> followers)
    {
        return followers.Any(f => f.FollowerId == currentUser.Id);
    }


    [UserMapping(Default = false)]
    private AvatarDto AvatarUrl(Guid userId)
    {
        return storageService.GetUserAvatar(userId);
    }
}