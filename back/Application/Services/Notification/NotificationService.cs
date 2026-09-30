using Application.Dtos.Notification;
using Application.Dtos.User;
using Application.Interfaces;
using Domain;
using Domain.Entities.Identity;
using Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Notification;

public class NotificationService(IAppDbContext db, INotifier notifier, IStorageService storage,
    ILogger<NotificationService> logger) : INotificationService
{
    public NotificationEntity? Create(Guid recipientId, Guid actorId, NotificationType type,
        Guid? resourceId = null, Guid? conversationId = null)
    {
        if (recipientId == actorId) return null;
        var notification = new NotificationEntity
        {
            RecipientId = recipientId, ActorId = actorId, Type = type,
            ResourceId = resourceId, ConversationId = conversationId
        };
        db.Notifications.Add(notification);
        return notification;
    }

    public async Task PublishAsync(IEnumerable<NotificationEntity?> notifications, CancellationToken ct = default)
    {
        try
        {
            foreach (var dto in await ToDtosAsync(notifications.OfType<NotificationEntity>(), ct))
                await notifier.SendNotificationAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Live notification delivery failed; notifications remain available in history");
        }
    }

    public async Task<List<NotificationDto>> ToDtosAsync(IEnumerable<NotificationEntity> notifications, CancellationToken ct = default)
    {
        var items = notifications.ToList();
        if (items.Count == 0) return [];
        var actorIds = items.Where(n => n.ActorId.HasValue).Select(n => n.ActorId!.Value).Distinct().ToList();
        var actors = await db.Set<UserEntity>().AsNoTracking().Where(u => actorIds.Contains(u.Id))
            .Select(u => new { u.Id, u.UserName }).ToDictionaryAsync(u => u.Id, ct);
        var commentIds = items.Where(n => n.Type is NotificationType.YourCommentLiked or NotificationType.YourCommentReplied)
            .Select(n => n.ResourceId).ToList();
        var comments = await db.Comments.AsNoTracking().Where(c => commentIds.Contains(c.Id))
            .Select(c => new { c.Id, c.VideoId }).ToDictionaryAsync(c => c.Id, ct);
        Guid? VideoId(NotificationEntity n) => n.Type switch
        {
            NotificationType.YourCommentLiked or NotificationType.YourCommentReplied =>
                n.ResourceId.HasValue && comments.TryGetValue(n.ResourceId.Value, out var comment) ? comment.VideoId : null,
            NotificationType.NewDMMessage or NotificationType.NewFollower => null,
            _ => n.ResourceId
        };
        var videoIds = items.Select(VideoId).Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        var videos = await db.Videos.AsNoTracking().Where(v => videoIds.Contains(v.Id))
            .Select(v => new { v.Id, v.ShortId }).ToDictionaryAsync(v => v.Id, ct);
        return items.Select(n => new NotificationDto
        {
            Id = n.Id, CreatedAt = n.CreatedAt, RecipientId = n.RecipientId, ActorId = n.ActorId,
            Type = n.Type, ResourceId = n.ResourceId, ConversationId = n.ConversationId, ReadAt = n.ReadAt,
            Actor = n.ActorId.HasValue && actors.TryGetValue(n.ActorId.Value, out var actor)
                ? new SimpleUserDto { Id = actor.Id, Username = actor.UserName ?? "", Avatar = storage.GetUserAvatar(actor.Id) } : null,
            VideoShortId = VideoId(n) is Guid videoId && videos.TryGetValue(videoId, out var video) ? video.ShortId : null
        }).ToList();
    }
}
