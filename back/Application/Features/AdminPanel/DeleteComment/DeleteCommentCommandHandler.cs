using Application.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.DeleteComment;

internal class DeleteCommentCommandHandler(IAppDbContext appDbContext)
    : IRequestHandler<DeleteCommentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await appDbContext.Comments
                          .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
                      ?? throw new NotFoundException(ErrorCodes.CommentNotFound);
        var videoId = comment.VideoId;
        var commentHierarchy = await appDbContext.Comments
            .Where(c => c.VideoId == videoId)
            .Select(c => new { c.Id, c.ParentCommentId })
            .ToListAsync(cancellationToken);
        var childrenByParent = commentHierarchy
            .Where(c => c.ParentCommentId.HasValue)
            .GroupBy(c => c.ParentCommentId!.Value)
            .ToDictionary(group => group.Key, group => group.Select(c => c.Id));
        var deletedCommentIds = new HashSet<Guid> { comment.Id };
        var pendingIds = new Queue<Guid>(deletedCommentIds);

        while (pendingIds.TryDequeue(out var parentId))
        {
            if (!childrenByParent.TryGetValue(parentId, out var childIds)) continue;
            foreach (var childId in childIds.Where(deletedCommentIds.Add))
            {
                pendingIds.Enqueue(childId);
            }
        }

        appDbContext.Comments.Remove(comment);
        await appDbContext.SaveChangesAsync(cancellationToken);
        await appDbContext.Videos
            .Where(v => v.Id == videoId)
            .ExecuteUpdateAsync(
                v => v.SetProperty(
                    x => x.CommentCount,
                    x => x.CommentCount > deletedCommentIds.Count
                        ? x.CommentCount - deletedCommentIds.Count
                        : 0),
                cancellationToken);
        return Unit.Value;
    }
}
