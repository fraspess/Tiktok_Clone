using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.GetFollowingFyp;

public record GetFollowingFypCommand(PaginationSettings PaginationSettings) : IRequest<PagedResult<VideoDto>>;