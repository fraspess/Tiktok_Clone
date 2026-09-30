using Application.Interfaces;
using Application.Services.Notification;
using Domain;
using Domain.Constants;
using Domain.Entities.Video;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Repost;

public class RepostVideoCommandHandler(
    IAppDbContext appDbContext,
    ICurrentUser currentUser,
    INotificationService notifications)
    : IRequestHandler<RepostVideoCommand, Unit>
{
    public async Task<Unit> Handle(RepostVideoCommand request, CancellationToken cancellationToken)
    {
        var exists = await appDbContext.VideoReposts
            .AnyAsync(u => u.UserId == currentUser.Id!.Value && u.VideoId == request.VideoId, cancellationToken);
        if (exists) return Unit.Value;

        var video = await appDbContext.Videos
                        .Select(v => new { v.Id, v.Author })
                        .FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken)
                    ?? throw new NotFoundException(ErrorCodes.VideoNotFound);

        var videoRepost = new VideoRepostEntity
        {
            UserId = currentUser.Id!.Value,
            VideoId = request.VideoId
        };

        appDbContext.VideoReposts.Add(videoRepost);

        var notification = notifications.Create(video.Author!.Id, currentUser.Id!.Value,
            NotificationType.YourVideoReposted, video.Id);
        await appDbContext.SaveChangesAsync(cancellationToken);
        await notifications.PublishAsync([notification], cancellationToken);

        return Unit.Value;
    }
}
