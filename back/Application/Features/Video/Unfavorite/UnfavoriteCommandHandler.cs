using System.Text.Unicode;
using Application.Extensions;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Unfavorite;

internal class UnfavoriteCommandHandler(IAppDbContext appDbContext) : IRequestHandler<UnfavoriteCommand, Unit>
{
    public async Task<Unit> Handle(UnfavoriteCommand request, CancellationToken cancellationToken)
    {
        var videoId = await appDbContext.Videos.GetIdFromShortIdAsync(request.VideoId, ct: cancellationToken);
        if (videoId == Guid.Empty) throw new NotFoundException("Відео не знайдено");
        
        var favorite = await appDbContext
            .Favorites
            .Where(f => f.Id == videoId)
            .FirstOrDefaultAsync(cancellationToken);
        if (favorite is null) return Unit.Value;
        
        appDbContext.Favorites.Remove(favorite);
        await appDbContext.SaveChangesAsync(cancellationToken);
        
        await appDbContext.Videos
            .Where(v => v.Id == videoId)
            .ExecuteUpdateAsync(v => v.SetProperty(x => x.FavoriteCount, x => x.FavoriteCount - 1),
                cancellationToken);

        return Unit.Value;
    }
}