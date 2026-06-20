using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;

namespace Application.Features.User.GetCurrentUser;

public class GetCurrentUserQueryHandler(
    UserManager<UserEntity> userManager,
    UserMapper mapper,
    IStorageService storageService,
    ICurrentUser currentUser,
    ICacheService cache)
    : IRequestHandler<GetCurrentUserQuery, UserMeDto>
{
    public async Task<UserMeDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"user:{currentUser.Id!}";
        var cached = await cache.GetAsync<UserMeDto>(cacheKey);
        if (cached is not null) return cached;

        var user = await userManager.Users
                       .Where(u => u.Id == currentUser.Id)
                       .ToProjectionDto(currentUser.Id)
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new NotFoundException("Користувача не знайдено?");

        var dto = mapper.ToMeDto(user);
        await cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(1));
        return dto;
    }
}