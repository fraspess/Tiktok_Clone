using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.GetUserVideos;

public class GetUserVideosQueryHandler(IAppDbContext appDbContext, VideoMapper videoMapper, ICurrentUser currentUser)
    : IRequestHandler<GetUserVideosQuery, PagedResult<VideoDto>>
{
    public async Task<PagedResult<VideoDto>> Handle(GetUserVideosQuery request, CancellationToken cancellationToken)
    {
        var videos = await appDbContext
            .Videos
            .Where(v => v.UserId == request.UserId)
            .OrderBy(v => v.CreatedAt)
            .ToProjectionDto(currentUser.Id)
            .ToPagedResultAsync(request.Settings);

        var result = videos.MapItems(videoMapper.ToDto);
        return result;
    }
}