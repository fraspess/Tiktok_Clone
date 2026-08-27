using Application.Interfaces;
using Domain.Constants;
using Domain.Entities.Video;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Repost;

public class RepostVideoCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser) : IRequestHandler<RepostVideoCommand, Unit>
{
    public async Task<Unit> Handle(RepostVideoCommand request, CancellationToken cancellationToken)
    {
        var exists = await appDbContext.VideoReposts
            .AnyAsync(u => u.UserId == currentUser.Id!.Value, cancellationToken: cancellationToken);
        if (exists) return Unit.Value;
        
        var existVideo = await appDbContext.Videos
            .AnyAsync(v => v.Id == request.VideoId, cancellationToken);
        
        if (!existVideo) throw new NotFoundException(ErrorCodes.VideoNotFound);
        var videoRepost = new VideoRepostEntity
        {
            UserId = currentUser.Id!.Value,
            VideoId = request.VideoId,
        };

        appDbContext.VideoReposts.Add(videoRepost);
        await appDbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}