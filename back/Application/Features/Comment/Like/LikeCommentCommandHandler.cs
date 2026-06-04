using Application.Interfaces;
using Domain.Entities.Comment;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comment.Like
{
    public class LikeCommentCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser) : IRequestHandler<LikeCommentCommand, Unit>
    {
        async Task<Unit> IRequestHandler<LikeCommentCommand, Unit>.Handle(LikeCommentCommand request,
            CancellationToken cancellationToken)
        {
            var comment = await appDbContext.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken: cancellationToken)
                          ?? throw new NotFoundException("Коментарій не знайдено");

            var isExists = comment.CommentLikes.FirstOrDefault(c => c.UserId == currentUser.Id);
            if (isExists is null)
            {
                isExists = new CommentLikeEntity() { CommentId = request.CommentId, UserId = currentUser.Id!.Value };
                comment.CommentLikes.Add(isExists);
            }
            else
            {
                comment.CommentLikes.Remove(isExists);
            }
            await appDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}