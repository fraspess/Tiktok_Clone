using Application.Interfaces;
using Domain.Entities.Comment;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Comment.Like
{
    public class LikeCommentCommandHandler(IUnitOfWork _uow, ICurrentUser currentUser) : IRequestHandler<LikeCommentCommand, Unit>
    {
        async Task<Unit> IRequestHandler<LikeCommentCommand, Unit>.Handle(LikeCommentCommand request,
            CancellationToken cancellationToken)
        {
            var comment = await _uow.Comments.GetByIdAsync(request.CommentId)
                          ?? throw new NotFoundException("Коментарій не знайдено");

            var isExists = comment.CommentLikes.FirstOrDefault(c => c.UserId == currentUser.Id);
            if (isExists is null)
            {
                isExists = new CommentLikeEntity() { CommentId = request.CommentId, UserId = currentUser.Id!.Value };
                comment.CommentLikes.Add(isExists);
                await _uow.SaveChangesAsync();
            }
            else
            {
                comment.CommentLikes.Remove(isExists);
                await _uow.SaveChangesAsync();
            }

            return Unit.Value;
        }
    }
}