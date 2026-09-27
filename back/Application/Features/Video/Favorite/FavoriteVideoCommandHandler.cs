using Application.Extensions;
using Application.Interfaces;
using Domain.Entities.Favorite;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Favorite;

internal class FavoriteVideoCommandHandler(IAppDbContext appDbContext, ICurrentUser user)
    : IRequestHandler<FavoriteVideoCommand, Unit>
{
    public async Task<Unit> Handle(FavoriteVideoCommand request, CancellationToken cancellationToken)
    {
        var videoId = request.VideoId;
        var userId = user.Id!.Value;

        var existingVideo = await appDbContext.Videos.GetIdFromShortIdAsync(videoId, ct: cancellationToken);
        if (existingVideo == Guid.Empty) throw new NotFoundException(ErrorCodes.Forbidden);

        var favoriteEntity = await appDbContext.Favorites.Where(f => f.UserId == userId && f.VideoId == existingVideo)
            .FirstOrDefaultAsync(cancellationToken);
        if (favoriteEntity != null) return Unit.Value;
        
            favoriteEntity = new FavoriteEntity
            {
                UserId = userId,
                VideoId = existingVideo
            };
            await appDbContext.Favorites.AddAsync(favoriteEntity, cancellationToken);

        await appDbContext.SaveChangesAsync(cancellationToken);
        
        await appDbContext.Videos
            .Where(v => v.Id == existingVideo)
            .ExecuteUpdateAsync(v => v.SetProperty(x => x.FavoriteCount, x => x.FavoriteCount + 1),
                cancellationToken);
        return Unit.Value;
    }
}