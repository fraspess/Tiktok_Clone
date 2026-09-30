using Application.Extensions;
using Application.Interfaces;
using Application.Services.Notification;
using Domain;
using Domain.Entities.Notification;
using Domain.Entities.Comment;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comment.Create;

internal class CreateCommentCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser, INotificationService notifications)
    : IRequestHandler<CreateCommentCommand, Unit>
{
    public async Task<Unit> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var videoId = await appDbContext.Videos.GetIdFromShortIdAsync(dto.VideoId, cancellationToken);
        if (videoId == Guid.Empty) throw new NotFoundException(ErrorCodes.VideoNotFound);

        var ownerId = currentUser.Id!.Value;
        var videoAuthorId = await appDbContext.Videos.Where(v => v.Id == videoId)
            .Select(v => v.UserId).SingleAsync(cancellationToken);
        var pending = new List<NotificationEntity?>();
        if (dto.ParentCommentId is not null)
        {
            var parent = await appDbContext.Comments.FirstOrDefaultAsync(
                c => c.Id == dto.ParentCommentId && c.VideoId == videoId, cancellationToken)
                ?? throw new NotFoundException(ErrorCodes.CommentNotFound);
            pending.Add(notifications.Create(parent.UserId, ownerId, NotificationType.YourCommentReplied, parent.Id));
            if (parent.UserId != videoAuthorId)
                pending.Add(notifications.Create(videoAuthorId, ownerId, NotificationType.YourVideoCommented, videoId));
            var newComment = new CommentEntity
            {
                Text = dto.Text, ParentCommentId = dto.ParentCommentId.Value, UserId = ownerId,
                VideoId = videoId
            };
            await appDbContext.Comments.AddAsync(newComment, cancellationToken);
        }
        else
        {
            pending.Add(notifications.Create(videoAuthorId, ownerId, NotificationType.YourVideoCommented, videoId));
            var comment = new CommentEntity { Text = dto.Text, UserId = ownerId, VideoId = videoId };
            await appDbContext.Comments.AddAsync(comment, cancellationToken);
        }

        await appDbContext.SaveChangesAsync(cancellationToken);
        await appDbContext.Videos
            .Where(v => v.Id == videoId)
            .ExecuteUpdateAsync(v => v.SetProperty(x => x.CommentCount, x => x.CommentCount + 1),
                cancellationToken);
        await notifications.PublishAsync(pending, cancellationToken);
        return Unit.Value;
    }
}
