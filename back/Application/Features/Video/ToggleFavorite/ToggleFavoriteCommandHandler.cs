using Application.Interfaces;
using Domain.Entities.Favorite;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.ToggleFavorite;

public class ToggleFavoriteCommandHandler(IAppDbContext appDbContext, ICurrentUser user)
    : IRequestHandler<ToggleFavoriteCommand, Unit>
{
    public async Task<Unit> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
    {
        var videoId = request.VideoId;
        var userId = user.Id!.Value;

        var existingVideo = await appDbContext.Videos.AnyAsync(v => v.Id == videoId, cancellationToken);
        if (!existingVideo) throw new NotFoundException("Відео не знайдено");

        var favoriteEntity = await appDbContext.Favorites.Where(f => f.UserId == userId && f.VideoId == videoId)
            .FirstOrDefaultAsync(cancellationToken);

        if (favoriteEntity is null)
        {
            favoriteEntity = new FavoriteEntity
            {
                UserId = userId,
                VideoId = videoId
            };
            await appDbContext.Favorites.AddAsync(favoriteEntity, cancellationToken);
            await appDbContext.Videos
                .Where(v => v.Id == videoId)
                .ExecuteUpdateAsync(v => v.SetProperty(x => x.FavoriteCount, x => x.FavoriteCount + 1),
                    cancellationToken);
        }
        else
        {
            appDbContext.Favorites.Remove(favoriteEntity);
            await appDbContext.Videos
                .Where(v => v.Id == videoId)
                .ExecuteUpdateAsync(v => v.SetProperty(x => x.FavoriteCount, x => x.FavoriteCount - 1),
                    cancellationToken);
        }

        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}