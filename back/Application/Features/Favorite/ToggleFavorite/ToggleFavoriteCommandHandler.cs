using Application.Interfaces;
using Domain.Entities.Favorite;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Favorite.ToggleFavorite
{
    public class ToggleFavoriteCommandHandler(IAppDbContext appDbContext, ICurrentUser user) : IRequestHandler<ToggleFavoriteCommand, Unit>
    {
        public async Task<Unit> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
        {
            var videoId = request.VideoId;
            var userId = user.Id!.Value;

            var video = await appDbContext.Videos.FirstOrDefaultAsync(v => v.Id == videoId, cancellationToken: cancellationToken)
                        ?? throw new NotFoundException("Відео не знайдено");

            var favoriteEntity = await appDbContext.Favorites.Where(f => f.UserId == userId && f.VideoId == videoId).FirstOrDefaultAsync(cancellationToken:cancellationToken);
            if (favoriteEntity is null)
            {
                favoriteEntity = new FavoriteEntity
                {
                    UserId = userId,
                    VideoId = videoId,
                };
                await appDbContext.Favorites.AddAsync(favoriteEntity, cancellationToken);
            }
            else
            {
                await appDbContext.Favorites.AddAsync(favoriteEntity, cancellationToken);
            }

            await appDbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}