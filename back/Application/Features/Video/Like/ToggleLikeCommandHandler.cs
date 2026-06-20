using Application.Interfaces;
using Domain.Entities.Video;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Like;

public class ToggleLikeCommandHandler(ICurrentUser user, IAppDbContext appDbContext)
    : IRequestHandler<ToggleLikeCommand, Unit>
{
    // Якщо є лайк забирає, нема - ставить
    public async Task<Unit> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        var existingLike =
            await appDbContext.VideoLikes.FirstOrDefaultAsync(l => l.UserId == user.Id && l.VideoId == request.VideoId,
                cancellationToken);

        var existingVideo = await appDbContext.Videos.AnyAsync(v => v.Id == request.VideoId, cancellationToken);
        if (!existingVideo) throw new NotFoundException("Відео не знайдено");

        if (existingLike is null)
        {
            await appDbContext.VideoLikes.AddAsync(new VideoLikeEntity
            {
                UserId = user.Id!.Value,
                VideoId = request.VideoId
            }, cancellationToken);

            await appDbContext.Videos
                .Where(v => v.Id == request.VideoId)
                .ExecuteUpdateAsync(v => v.SetProperty(x => x.LikeCount, x => x.LikeCount + 1), cancellationToken);
        }
        else
        {
            appDbContext.VideoLikes.Remove(existingLike);
            await appDbContext.Videos
                .Where(v => v.Id == request.VideoId)
                .ExecuteUpdateAsync(v => v.SetProperty(x => x.LikeCount, x => x.LikeCount - 1), cancellationToken);
        }

        // EF слідкує після FirstOrDefaultAsync()
        // appDbContext.Videos.Update(video);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}