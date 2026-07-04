using Application.Interfaces;
using Domain.Entities.Video;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Like;

internal class LikeVideoCommandHandler(ICurrentUser user, IAppDbContext appDbContext)
    : IRequestHandler<LikeVideoCommand, Unit>
{
    public async Task<Unit> Handle(LikeVideoCommand request, CancellationToken cancellationToken)
    {
        var videoId = await appDbContext.Videos.Where(v => v.ShortId == request.VideoId).Select(v => v.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (videoId == Guid.Empty) throw new NotFoundException(ErrorCodes.VideoNotFound);
        
        var existingLike =
            await appDbContext.VideoLikes.AnyAsync(l => l.UserId == user.Id && l.VideoId == videoId,
                cancellationToken);
        if (existingLike) return Unit.Value;
        

        await appDbContext.VideoLikes.AddAsync(new VideoLikeEntity
        {
            UserId = user.Id!.Value,
            VideoId = videoId
        }, cancellationToken);
        
        await appDbContext.SaveChangesAsync(cancellationToken);
        await appDbContext.Videos
            .Where(v => v.Id == videoId)
            .ExecuteUpdateAsync(v => v.SetProperty(x => x.LikeCount, x => x.LikeCount + 1), cancellationToken);
        return Unit.Value;
    }
}