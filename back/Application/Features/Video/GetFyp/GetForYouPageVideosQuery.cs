using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.GetFyp;

public record GetForYouPageVideosQuery(PaginationSettings PaginationSettings)
    : IRequest<PagedResult<VideoDto>>;