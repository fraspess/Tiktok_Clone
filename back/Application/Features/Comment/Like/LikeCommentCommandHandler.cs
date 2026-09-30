using Application.Interfaces;
using Domain.Entities.Comment;
using Domain.Constants;
using Domain;
using Application.Services.Notification;
using Domain.Entities.Notification;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comment.Like;

public class LikeCommentCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser, INotificationService notifications)
    : IRequestHandler<LikeCommentCommand, Unit>
{
    async Task<Unit> IRequestHandler<LikeCommentCommand, Unit>.Handle(LikeCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await appDbContext.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId,
            cancellationToken) ?? throw new NotFoundException(ErrorCodes.CommentNotFound);

        var isExists =
            await appDbContext.CommentLikes.FirstOrDefaultAsync(
                c => c.UserId == currentUser.Id && c.CommentId == request.CommentId, cancellationToken);

        NotificationEntity? notification = null;
        if (isExists is null)
        {
            isExists = new CommentLikeEntity { CommentId = request.CommentId, UserId = currentUser.Id!.Value };
            appDbContext.CommentLikes.Add(isExists);
            notification = notifications.Create(comment.UserId, currentUser.Id!.Value,
                NotificationType.YourCommentLiked, comment.Id);
        }
        else
        {
            appDbContext.CommentLikes.Remove(isExists);
        }

        await appDbContext.SaveChangesAsync(cancellationToken);
        await notifications.PublishAsync([notification], cancellationToken);

        return Unit.Value;
    }
}
