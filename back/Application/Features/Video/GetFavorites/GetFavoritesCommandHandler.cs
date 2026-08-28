using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.GetFavorites;

public class GetFavoritesCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser, VideoMapper videoMapper) : IRequestHandler<GetFavoritesCommand, PagedResult<SimpleVideoDto>>
{
    public async Task<PagedResult<SimpleVideoDto>> Handle(GetFavoritesCommand request, CancellationToken cancellationToken)
    {
        var favoritesVideo = await appDbContext.Videos
            .Where(v => v.Favorites.Any(f => f.UserId == request.userId))
            .ToProjectionDto(currentUser.Id)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        var mapped = favoritesVideo.MapItems(videoMapper.ToSimpleDto);
        return mapped;
    }
}