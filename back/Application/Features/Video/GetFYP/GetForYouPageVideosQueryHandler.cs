using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Video.GetFYP
{
    public class GetForYouPageVideosQueryHandler(IUnitOfWork _uow, VideoMapper videoMapper, IConfiguration config, ICurrentUser currentUser)
        : IRequestHandler<GetForYouPageVideosQuery, PagedResult<VideoDto>>
    {
        public async Task<PagedResult<VideoDto>> Handle(GetForYouPageVideosQuery request,
            CancellationToken cancellationToken)
        {
            var videos = await _uow.Videos
                .GetAll()
                .OrderBy(v => Guid.NewGuid())
                .ToProjectionDto(currentUser.Id)
                .ToPagedResultAsync(request.PaginationSettings);
            
            var result = videos.MapItems(videoMapper.ToDto);
            return result;
        }
    }
}