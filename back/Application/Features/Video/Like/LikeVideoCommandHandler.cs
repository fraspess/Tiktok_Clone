using Application.Interfaces;
using Application.Mapper;
using Domain;
using Domain.Constants;
using Domain.Entities.Notification;
using Domain.Entities.Video;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Video.Like;

internal class LikeVideoCommandHandler(
    ICurrentUser user,
    IAppDbContext appDbContext,
    INotifier notifier,
    NotificationMapper mapper)
    : IRequestHandler<LikeVideoCommand, Unit>
{
    public async Task<Unit> Handle(LikeVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await appDbContext.Videos.Where(v => v.ShortId == request.VideoId)
                        .Select(v => new { v.Id, v.Author }).FirstOrDefaultAsync(cancellationToken)
                    ?? throw new NotFoundException(ErrorCodes.VideoNotFound);

        var existingLike =
            await appDbContext.VideoLikes.AnyAsync(l => l.UserId == user.Id && l.VideoId == video.Id,
                cancellationToken);
        if (existingLike) return Unit.Value;


        await appDbContext.VideoLikes.AddAsync(new VideoLikeEntity
        {
            UserId = user.Id!.Value,
            VideoId = video.Id
        }, cancellationToken);
        
        var notification = new NotificationEntity
        {
            ActorId = user.Id,
            RecipientId = video.Author!.Id,
            Type = NotificationType.YourVideoLiked,
            ResourceId = video.Id
        };
        appDbContext.Notifications.Add(notification);
        await appDbContext.SaveChangesAsync(cancellationToken);
        
        await appDbContext.Videos
            .Where(v => v.Id == video.Id)
            .ExecuteUpdateAsync(v => v.SetProperty(x => x.LikeCount, x => x.LikeCount + 1), cancellationToken);
        
        var dto = mapper.ToDto(notification);
        await notifier.SendNotificationAsync(dto);
        return Unit.Value;
    }
}