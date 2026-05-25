using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Pagination;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Video.MyVideos
{
    internal class GetMyVideosQueryHandler(IUnitOfWork _uow, IMapper _mapper, IConfiguration config, ICurrentUser currentUser)
        : IRequestHandler<GetMyVideosQuery, PagedResult<MyVideoDTO>>
    {
        public Task<PagedResult<MyVideoDTO>> Handle(GetMyVideosQuery request, CancellationToken cancellationToken)
        {
            var videos = _uow.Videos
                //.GetAllIgnoreQueryFilters()
                .GetAll()
                .Where(v => v.UserId == currentUser.Id!.Value)
                .OrderByDescending(v => v.CreatedAt)
                .ProjectTo<MyVideoDTO>(_mapper.ConfigurationProvider,
                    new { userId = currentUser.Id!.Value , backendUrl = config["Backend:Url"]})
                .ToPagedResultAsync(request.Settings);
            return videos;
        }
    }
}