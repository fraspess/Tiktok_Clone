using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.GetFollowers;

internal class GetUserFollowersCommandHandler(UserManager<UserEntity> userManager, IStorageService storageService)
    : IRequestHandler<GetUserFollowersCommand, PagedResult<SimpleUserDto>>
{
    public async Task<PagedResult<SimpleUserDto>> Handle(GetUserFollowersCommand request,
        CancellationToken cancellationToken)
    {
        var followers = await userManager
            .Users
            .Where(u => u.UserName == request.Username)
            .SelectMany(u => u.Followers)
            .Select(f => new { f.FollowerId, f.Follower.UserName })
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken);

        return followers.MapItems(f => new SimpleUserDto
        {
            Id = f.FollowerId,
            Username = f.UserName!,
            Avatar = storageService.GetUserAvatar(f.FollowerId)
        });
    }
}