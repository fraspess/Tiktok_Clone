using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.MyVideos
{
    public record GetMyVideosQuery(PaginationSettings Settings) : IRequest<PagedResult<MyVideoDTO>>;
}