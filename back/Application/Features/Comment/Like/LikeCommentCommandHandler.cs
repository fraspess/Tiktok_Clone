using Application.Interfaces;
using Domain.Entities.Comment;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comment.Like;

public class LikeCommentCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser)
    : IRequestHandler<LikeCommentCommand, Unit>
{
    async Task<Unit> IRequestHandler<LikeCommentCommand, Unit>.Handle(LikeCommentCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await appDbContext.Comments.AnyAsync(c => c.Id == request.CommentId,
            cancellationToken);
        if (!exists) throw new NotFoundException(ErrorCodes.CommentNotFound);

        var isExists =
            await appDbContext.CommentLikes.FirstOrDefaultAsync(
                c => c.UserId == currentUser.Id || c.CommentId == request.CommentId, cancellationToken);

        if (isExists is null)
        {
            isExists = new CommentLikeEntity { CommentId = request.CommentId, UserId = currentUser.Id!.Value };
            appDbContext.CommentLikes.Add(isExists);
        }
        else
        {
            appDbContext.CommentLikes.Remove(isExists);
        }

        await appDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}