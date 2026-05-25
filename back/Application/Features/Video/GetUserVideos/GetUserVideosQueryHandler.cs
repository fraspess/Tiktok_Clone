using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Pagination;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.Features.Video.GetUserVideos
{
    public class GetUserVideosQueryHandler(IUnitOfWork _uow, IMapper _mapper, ICurrentUser currentUser)
        : IRequestHandler<GetUserVideosQuery, PagedResult<VideoDTO>>
    {
        public async Task<PagedResult<VideoDTO>> Handle(GetUserVideosQuery request, CancellationToken cancellationToken)
        {
            var videos = await _uow.Videos
                .GetAll()
                .Where(v => v.UserId == request.UserId)
                .OrderBy(v => v.CreatedAt)
                .ProjectTo<VideoDTO>(_mapper.ConfigurationProvider, new { currentUserId = currentUser.Id })
                .ToPagedResultAsync(request.Settings);
            return videos;
        }
    }
}