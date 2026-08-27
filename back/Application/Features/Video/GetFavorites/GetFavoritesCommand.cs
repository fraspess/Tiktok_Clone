using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.GetFavorites;

public record GetFavoritesCommand(Guid userId, PaginationSettings PaginationSettings) : IRequest<PagedResult<SimpleVideoDto>>;