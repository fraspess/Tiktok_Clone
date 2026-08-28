using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.User.GetLikedVideos;

public record GetLikedVideosCommand(Guid UserId, PaginationSettings PaginationSettings) : IRequest<PagedResult<SimpleVideoDto>>;