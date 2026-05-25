using Application.Interfaces;
using Domain.Entities.Favorite;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Favorite.ToggleFavorite
{
    public class ToggleFavoriteCommandHandler(IUnitOfWork _uow, ICurrentUser user) : IRequestHandler<ToggleFavoriteCommand, Unit>
    {
        public async Task<Unit> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
        {
            var videoId = request.VideoId;
            var userId = user.Id!.Value;

            var video = await _uow.Videos.GetByIdAsync(videoId)
                        ?? throw new NotFoundException("Відео не знайдено");

            var favoriteEntity = _uow.Favorites.GetByVideoAndUserIds(videoId, userId);
            if (favoriteEntity is null)
            {
                favoriteEntity = new FavoriteEntity
                {
                    UserId = userId,
                    VideoId = videoId,
                };
                await _uow.Favorites.CreateAsync(favoriteEntity);
            }
            else
            {
                await _uow.Favorites.DeleteAsync(favoriteEntity);
            }

            await _uow.SaveChangesAsync();
            return Unit.Value;
        }
    }
}