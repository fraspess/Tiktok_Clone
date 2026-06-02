using Application.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.AdminPanel.DeleteComment;

internal class DeleteCommentCommandHandler(IUnitOfWork _uow) : IRequestHandler<DeleteCommentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _uow.Comments
                          .GetByIdAsync(request.Id)
                      ?? throw new NotFoundException("Коментарій не знайдено");
        comment.IsDeleted = true;
        await _uow.Comments.UpdateAsync(comment);
        await _uow.SaveChangesAsync();
        return Unit.Value;
    }
}