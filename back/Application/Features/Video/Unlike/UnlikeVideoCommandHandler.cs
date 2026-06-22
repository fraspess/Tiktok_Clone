using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.UnLike;

public class UnlikeVideoCommandHandler(IAppDbContext appDbContext, ICurrentUser user) : IRequestHandler<UnlikeVideoCommand, Unit>
{
    public async Task<Unit> Handle(UnlikeVideoCommand request, CancellationToken cancellationToken)
    {
        var videoId = await appDbContext.Videos.Where(v => v.ShortId == request.VideoId).Select(v => v.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (videoId == default) throw new NotFoundException("Відео не знайдено");

        var existingLike =
            await appDbContext.VideoLikes.FirstOrDefaultAsync(l => l.UserId == user.Id && l.VideoId == videoId,
                cancellationToken);
        if (existingLike is null) return Unit.Value;
        
        appDbContext.VideoLikes.Remove(existingLike);
        await appDbContext.SaveChangesAsync(cancellationToken);
        await appDbContext
            .Videos
            .Where(v => v.Id == videoId)
            .ExecuteUpdateAsync(v => v.SetProperty(x => x.LikeCount, x => x.LikeCount - 1), cancellationToken);
        return Unit.Value;
    }
}