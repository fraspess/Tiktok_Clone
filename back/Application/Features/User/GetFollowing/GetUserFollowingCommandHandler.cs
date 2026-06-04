using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Pagination;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.GetFollowing;

public class GetUserFollowingCommandHandler(UserManager<UserEntity> userManager, IStorageService storageService) : IRequestHandler<GetUserFollowingCommand, PagedResult<SimpleUserDto>>
{
    public async Task<PagedResult<SimpleUserDto>> Handle(GetUserFollowingCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Where(u => u.UserName == request.Username)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        Console.WriteLine($"Found user: {user?.Id}");
        var following = await userManager
            .Users
            .Where(u => u.UserName == request.Username)
            .SelectMany(u => u.Following)
            .Select(f => new { f.FollowingId, f.Following.UserName })
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);
        
        return following.MapItems(f => new SimpleUserDto()
        {
            Id = f.FollowingId,
            Username = f.UserName!,
            AvatarUrl = storageService.GetUserAvatar(f.FollowingId),
        });
    }
}