using Application.Dtos.User;
using Application.Features.AdminPanel.GetUserById;
using AutoMapper;
using Domain.Entities.Identity;
using Org.BouncyCastle.Crypto.Generators;

namespace Application.MapperProfiles.User;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        string? backendUrl = null;
        CreateMap<UserEntity, UserDTO>()
            .ForMember(u => u.FollowersCount,
                o => o.Ignore())
            .ForMember(u => u.FollowingCount,
                o => o.Ignore())
            .ForMember(u => u.IsOwnProfile,
                o => o.Ignore())
            .ForMember(u => u.IsFollowing,
                o => o.Ignore())
            .ForMember(u => u.Username,
                o => o.MapFrom(u => $"@{u.UserName}"))
            .ForMember(u => u.Avatar, o => o.MapFrom(u => $"{backendUrl}/images/{u.Avatar}"));;

        CreateMap<UserEntity, UserMeDTO>()
            .ForMember(u => u.FollowersCount,
                o => o.Ignore())
            .ForMember(u => u.FollowingCount,
                o => o.Ignore())
            .ForMember(u => u.Username,
                o => o.MapFrom(u => $"@{u.UserName}"))
            .ForMember(u => u.Avatar, o => o.MapFrom(u => $"{backendUrl}/images/{u.Avatar}"));;

        CreateMap<RegisterUserDTO, UserEntity>()
            .ForMember(dest => dest.Avatar, opt => opt.Ignore());

        CreateMap<UserEntity, UserAuthorDTO>()
            .ForMember(u => u.Username, o => o.MapFrom(u => $"@{u.UserName}"))
            .ForMember(u => u.Avatar, o => o.MapFrom(u => $"{backendUrl}/images/{u.Avatar}"));;

        CreateMap<UserEntity, SimpleUserDTO>()
            .ForMember(u => u.Username,
                o => o.MapFrom(u => $"@{u.UserName}"))
            .ForMember(u => u.Avatar, o => o.MapFrom(u => $"{backendUrl}/images/{u.Avatar}"));;
        
        CreateMap<UserEntity, GetUserAdminDTO>()
            .ForMember(u => u.Username,
                o => o.MapFrom(u => $"@{u.UserName}"))
            .ForMember(u => u.Avatar, o => o.MapFrom(u => $"{backendUrl}/images/{u.Avatar}"));
    }
}