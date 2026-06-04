using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Video.MyVideos
{
    internal class GetMyVideosQueryHandler(IAppDbContext appDbContext, VideoMapper videoMapper, IConfiguration config, ICurrentUser currentUser)
        : IRequestHandler<GetMyVideosQuery, PagedResult<MyVideoDto>>
    {
        public async Task<PagedResult<MyVideoDto>> Handle(GetMyVideosQuery request, CancellationToken cancellationToken)
        {
            var videos = await appDbContext
                .Videos
                .Where(v => v.UserId == currentUser.Id!.Value)
                .OrderByDescending(v => v.CreatedAt)
                .ToProjectionDto(currentUser.Id)
                .ToPagedResultAsync(request.Settings);

            var result = videos.MapItems(videoMapper.ToMyDto);
            return result;
        }
    }
}