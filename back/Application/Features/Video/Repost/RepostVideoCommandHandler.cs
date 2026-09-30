using Application.Interfaces;
using Application.Mapper;
using Domain;
using Domain.Constants;
using Domain.Entities.Notification;
using Domain.Entities.Video;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Repost;

public class RepostVideoCommandHandler(
    IAppDbContext appDbContext,
    ICurrentUser currentUser,
    INotifier notifier,
    NotificationMapper notificationMapper)
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

        var notification = new NotificationEntity
        {
            RecipientId = video.Author!.Id,
            ActorId = currentUser.Id,
            ResourceId = video.Id,
            Type = NotificationType.YourVideoReposted
        };

        appDbContext.Notifications.Add(notification);
        await appDbContext.SaveChangesAsync(cancellationToken);
        var dto = notificationMapper.ToDto(notification);
        await notifier.SendNotificationAsync(dto);

        return Unit.Value;
    }
}