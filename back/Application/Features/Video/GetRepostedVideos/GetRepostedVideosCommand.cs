using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.GetRepostedVideos;

public record GetRepostedVideosCommand(Guid UserId, PaginationSettings PaginationSettings) : IRequest<PagedResult<SimpleVideoDto>>;