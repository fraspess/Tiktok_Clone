using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Video.GetFyp;

public class GetForYouPageVideosQueryHandler(
    IAppDbContext appDbContext,
    VideoMapper videoMapper,
    IConfiguration config,
    ICurrentUser currentUser)
    : IRequestHandler<GetForYouPageVideosQuery, PagedResult<VideoDto>>
{
    public async Task<PagedResult<VideoDto>> Handle(GetForYouPageVideosQuery request,
        CancellationToken cancellationToken)
    {
        var videos = await appDbContext
            .Videos
            .OrderBy(v => v.Id)
            .ToProjectionDto(currentUser.Id)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        var result = videos.MapItems(videoMapper.ToDto);
        return result;
    }
}