using Application.Extensions;
using Application.Interfaces;
using Domain.Entities.Video;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.View;

public class ViewVideoCommandHandler(IAppDbContext context, ICurrentUser user) : IRequestHandler<ViewVideoCommand, Unit>
{
    public async Task<Unit> Handle(ViewVideoCommand request, CancellationToken cancellationToken)
    {
        var videoId = await context.Videos.GetIdFromShortIdAsync(request.Id, cancellationToken);
        if (videoId == Guid.Empty) throw new NotFoundException(ErrorCodes.VideoNotFound);

        // Anonymous playback is counted without creating a user-owned view record.
        if (user.Id is null)
        {
            await context.Videos.Where(v => v.Id == videoId)
                .ExecuteUpdateAsync(v => v.SetProperty(x => x.ViewCount, x => x.ViewCount + 1), cancellationToken);
            return Unit.Value;
        }

        var existingView = await context
            .VideoViews
            .Where(v => v.VideoId == videoId && user.Id == v.UserId)
            .OrderByDescending(v => v.ViewedAt)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (existingView is not null)
        {
            if (!(DateTime.UtcNow - existingView.ViewedAt > TimeSpan.FromHours(1)))
            {
                return Unit.Value;
            }   
        }
        
        var newView = new VideoViewEntity()
        {
            UserId = user.Id!.Value,
            VideoId = videoId,
            ViewedAt = DateTime.UtcNow
        };
        
        await context.VideoViews.AddAsync(newView, cancellationToken);
        await context
            .Videos
            .Where(v => v.Id == videoId)
            .ExecuteUpdateAsync(v => v.SetProperty(x => x.ViewCount, x => x.ViewCount + 1), cancellationToken: cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
