using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.GetBySomeQuery
{
    public record GetVideosBySomeStringQuery(string SomeString, PaginationSettings Settings)
        : IRequest<PagedResult<SimpleVideoDto>>;
}