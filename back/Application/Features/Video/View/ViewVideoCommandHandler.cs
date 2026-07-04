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

        var existingView = await context
            .VideoViews
            .Where(v => v.VideoId == videoId && user.Id == v.UserId)
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