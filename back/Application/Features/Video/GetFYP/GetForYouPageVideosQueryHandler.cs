using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Pagination;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Video.GetFYP
{
    public class GetForYouPageVideosQueryHandler(IUnitOfWork _uow, IMapper _mapper, IConfiguration config, ICurrentUser currentUser)
        : IRequestHandler<GetForYouPageVideosQuery, PagedResult<VideoDTO>>
    {
        public async Task<PagedResult<VideoDTO>> Handle(GetForYouPageVideosQuery request,
            CancellationToken cancellationToken)
        {
            var videos = await _uow.Videos
                .GetAll()
                .OrderBy(v => Guid.NewGuid())
                .ProjectTo<VideoDTO>(_mapper.ConfigurationProvider, new { currentUserId = currentUser.Id, backendUrl = config["Backend:Url"] })
                .ToPagedResultAsync(request.PaginationSettings);

            return videos;
        }
    }
}