using Application.Dtos.User;
using Application.Features.AdminPanel.GetUserById;
using Application.Interfaces;
using Domain.Entities.Identity;
using Riok.Mapperly.Abstractions;

namespace Application.Mapper;

[Mapper]
public partial class UserMapper(IStorageService storageService, ICurrentUser currentUser)
{
    
    [MapProperty(nameof(UserProjectionDto.Username), nameof(UserMeDto.Username), Use = nameof(AddUsernameAtSymbol))]
    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserMeDto.Avatar), Use = nameof(AvatarUrl))]
    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserMeDto.IsOwnProfile), Use = nameof(IsOwnProfile))]
    public partial UserMeDto ToMeDto(UserProjectionDto user);
    
    [MapProperty(nameof(UserProjectionDto.Username), nameof(UserDto.Username), Use = nameof(AddUsernameAtSymbol))]
    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserDto.Avatar), Use = nameof(AvatarUrl))]
    [MapProperty(nameof(UserProjectionDto.Id), nameof(UserDto.IsOwnProfile), Use = nameof(IsOwnProfile))]
    public partial UserDto ToDto(UserProjectionDto user);

    [MapProperty(nameof(RegisterUserDto.Username), nameof(UserEntity.UserName), Use =  nameof(AddUsernameAtSymbol))]
    public partial UserEntity ToEntity(RegisterUserDto dto);
    
    [MapProperty(nameof(UserEntity.UserName), nameof(GetUserAdminDto.Username), Use =  nameof(AddUsernameAtSymbol))]
    public partial GetUserAdminDto ToGetUserAdminDto(UserEntity source);
    
    [MapProperty(nameof(UserProjectionDto.Id), nameof(SimpleUserDto.Avatar),Use = nameof(AvatarUrl))]
    public partial SimpleUserDto ToSimpleDto(UserProjectionDto source);
    
    [UserMapping(Default = false)]
    private string AddUsernameAtSymbol(string username) =>
        username.StartsWith("@") ? username : $"@{username}";
    
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