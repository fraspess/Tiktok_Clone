using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.GetFollowingFYP;

public record GetFollowingFYPCommand(PaginationSettings PaginationSettings) : IRequest<PagedResult<VideoDto>>;