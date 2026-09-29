using Application.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comment.Delete;

public class DeleteCommentCommandHandler(IAppDbContext appDbContext, ICurrentUser user)
    : IRequestHandler<DeleteCommentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await appDbContext.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken)
                      ?? throw new NotFoundException(ErrorCodes.CommentNotFound);
        var count = 1;
        if (comment.UserId == user.Id)
        {
            count = appDbContext.Comments.Count(c => c.ParentCommentId == request.CommentId);
            appDbContext.Comments.Remove(comment);
        }
        else
        {
            throw new NotAllowedException(ErrorCodes.Forbidden);
        }

        await appDbContext.SaveChangesAsync(cancellationToken);
        await appDbContext
            .Videos
            .Where(v => v.Id == comment.VideoId)
            .ExecuteUpdateAsync(v => v.SetProperty(x => x.CommentCount, x => x.CommentCount - count), cancellationToken: cancellationToken);
        return Unit.Value;
    }
}