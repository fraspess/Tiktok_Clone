using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comment.Delete
{
    public class DeleteCommentCommandHandler(IAppDbContext appDbContext, ICurrentUser user) : IRequestHandler<DeleteCommentCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await appDbContext.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken: cancellationToken)
                          ?? throw new ValidationException("Коментарій не знайдено");
            if (comment.UserId == user.Id)
            {
                appDbContext.Comments.Remove(comment);
            }
            else
            {
                throw new NotAllowedException("Ви не маєте прав на це");
            }

            await appDbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}