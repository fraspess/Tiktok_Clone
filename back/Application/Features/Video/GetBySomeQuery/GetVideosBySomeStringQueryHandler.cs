using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Video.GetBySomeQuery
{
    public class GetVideosBySomeStringQueryHandler(IUnitOfWork _uow, ICurrentUser currentUser, VideoMapper videoMapper)
        : IRequestHandler<GetVideosBySomeStringQuery, PagedResult<SimpleVideoDto>>
    {
        public async Task<PagedResult<SimpleVideoDto>> Handle(GetVideosBySomeStringQuery request,
            CancellationToken cancellationToken)
        {
            var someString = request.SomeString.ToLower().Trim();
            var query = _uow.Videos
                .GetAll()
                .Include(v => v.HashTags)
                .Include(v => v.Author)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(someString))
            {
                query = query.Where(v =>
                    v.Description.ToLower().Contains(someString) ||
                    v.Author!.UserName!.ToLower().Contains(someString) ||
                    v.HashTags.Any(h => h.HashTag.Tag.ToLower().Contains(someString))
                );
            }

            var videos = await query
                .OrderByDescending(v => v.CreatedAt)
                .ToProjectionDto(currentUser.Id)
                .ToPagedResultAsync(request.Settings);

            var result = videos.MapItems(videoMapper.ToSimpleDto);
            return result;
        }
    }
}