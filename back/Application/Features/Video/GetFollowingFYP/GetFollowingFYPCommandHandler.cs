using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.GetFollowingFYP;

public class GetFollowingFYPCommandHandler(IAppDbContext context, ICurrentUser currentUser, VideoMapper videoMapper)
    : IRequestHandler<GetFollowingFYPCommand, PagedResult<VideoDto>>
{
    public async Task<PagedResult<VideoDto>> Handle(GetFollowingFYPCommand request, CancellationToken cancellationToken)
    {
        var followingIds = await context
            .UserFollows
            .Where(f => f.FollowerId == currentUser.Id)
            .Select(f => f.FollowingId)
            .ToHashSetAsync(cancellationToken);

        var videos = await context
            .Videos
            .Where(v => followingIds.Contains(v.UserId))
            .ToProjectionDto(currentUser.Id!)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken);

        return videos.MapItems(videoMapper.ToDto);
    }
}