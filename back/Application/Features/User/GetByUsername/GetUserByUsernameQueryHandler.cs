using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.GetByUsername
{
    public class GetUserByUsernameQueryHandler(IUserService service, UserManager<UserEntity> userManager, UserMapper userMapper, ICurrentUser currentUser, ICacheService cache) : IRequestHandler<GetUserByUsernameQuery, UserDto>
    {
        public async Task<UserDto> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"user:username:{request.Username}";
            var cached =  await cache.GetAsync<UserDto>(cacheKey);
            if (cached is not null) return cached;
            var user = await userManager.Users.ToProjectionDto(currentUser.Id).FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken: cancellationToken)
                ?? throw new NotFoundException("Користувача не знайдено");
            var dto = userMapper.ToDto(user);
            await cache.SetAsync(cacheKey, dto);
            return dto;
        }
    }
}