using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.GetUserVideos
{
    public record GetUserVideosQuery(Guid UserId, PaginationSettings Settings)
        : IRequest<PagedResult<VideoDto>>;
}