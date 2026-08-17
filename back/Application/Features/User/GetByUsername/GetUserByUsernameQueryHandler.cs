using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entities.Identity;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.GetByUsername;

public class GetUserByUsernameQueryHandler(
    IUserService service,
    UserManager<UserEntity> userManager,
    UserMapper userMapper,
    ICurrentUser currentUser,
    ICacheService cache) : IRequestHandler<GetUserByUsernameQuery, UserDto>
{

public async Task<UserDto> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken){
        // IMPORTANT: the DTO below is personalized per viewer (IsOwnProfile, IsFollowing
        // depend on currentUser.Id), so the cache key MUST include the viewer's identity.
        // Caching only by username would let whichever user hits the DB first "poison"
        // the cache for every other visitor of that profile with their own personal state
        // (e.g. someone else seeing an "Edit profile" button, or a wrong follow status).
        var viewerKey = currentUser.Id?.ToString() ?? "anon";
        var cacheKey = $"user:username:{request.Username}:viewer:{viewerKey}";
        var cached = await cache.GetAsync<UserDto>(cacheKey);
        if (cached is not null) return cached;
        var user = await userManager.Users.ToProjectionDto(currentUser.Id)
                       .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken)
                   ?? throw new NotFoundException(ErrorCodes.UserNotFound);
        var dto = userMapper.ToDto(user);
        await cache.SetAsync(cacheKey, dto);
        return dto;
	}
}