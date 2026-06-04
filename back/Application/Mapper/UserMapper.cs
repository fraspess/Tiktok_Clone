using Application.Dtos.User;
using Application.Features.AdminPanel.GetUserById;
using Application.Interfaces;
using Domain.Entities.Identity;
using Riok.Mapperly.Abstractions;

namespace Application.Mapper;

[Mapper]
public partial class UserMapper(IStorageService storageService, ICurrentUser currentUser)
{
    
    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserMeDto.AvatarUrl), Use = nameof(AvatarUrl))]
    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserMeDto.IsOwnProfile), Use = nameof(IsOwnProfile))]
    public partial UserMeDto ToMeDto(UserProjectionDto user);
    
    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserDto.AvatarUrl), Use = nameof(AvatarUrl))]
    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserDto.IsOwnProfile), Use = nameof(IsOwnProfile))]
    public partial UserDto ToDto(UserProjectionDto user);
    
    public partial UserEntity ToEntity(RegisterUserDto dto);

    public partial GetUserAdminDto ToGetUserAdminDto(UserEntity source);
    
    [MapProperty(nameof(UserProjectionDto.Id), nameof(SimpleUserDto.AvatarUrl),Use = nameof(AvatarUrl))]
    public partial SimpleUserDto ToSimpleDto(UserProjectionDto source);
    
    
    [UserMapping(Default = false)]
    private bool IsOwnProfile(Guid userId) =>
        currentUser.Id == userId;
    
    [UserMapping(Default = false)]
    private bool IsFollowing(ICollection<UserFollowEntity> followers) =>
        followers.Any(f => f.FollowerId == currentUser.Id);

        
    [UserMapping(Default = false)]
    private string AvatarUrl(Guid userId) =>
        storageService.GetUserAvatar(userId);
    
}