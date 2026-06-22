using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.DeleteComment;

internal class DeleteCommentCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser)
    : IRequestHandler<DeleteCommentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await appDbContext.Comments
                          .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
                      ?? throw new NotFoundException("Коментарій не знайдено");
        appDbContext.Comments.Remove(comment);
        ;
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}