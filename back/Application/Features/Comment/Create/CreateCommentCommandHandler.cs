using Application.Interfaces;
using Domain.Entities.Comment;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comment.Create;

public class CreateCommentCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser)
    : IRequestHandler<CreateCommentCommand, Unit>
{
    public async Task<Unit> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var existingVideo = await appDbContext.Videos.AnyAsync(v => v.Id == dto.VideoId, cancellationToken);
        if (!existingVideo) throw new NotFoundException("Відео не знайдено");

        var ownerId = currentUser.Id!.Value;
        if (dto.ParentCommentId is not null)
        {
            var exists =
                await appDbContext.Comments.AnyAsync(c => c.Id == dto.ParentCommentId, cancellationToken);
            if (!exists) throw new NotFoundException("Коментарій не знайдено");
            var newComment = new CommentEntity
            {
                Text = dto.Text, ParentCommentId = dto.ParentCommentId.Value, UserId = ownerId,
                VideoId = dto.VideoId
            };
            await appDbContext.Comments.AddAsync(newComment, cancellationToken);
            await appDbContext.Videos
                .Where(v => v.Id == dto.VideoId)
                .ExecuteUpdateAsync(v => v.SetProperty(x => x.CommentCount, x => x.CommentCount + 1),
                    cancellationToken);
        }
        else
        {
            var comment = new CommentEntity { Text = dto.Text, UserId = ownerId, VideoId = dto.VideoId };
            await appDbContext.Comments.AddAsync(comment, cancellationToken);
            await appDbContext.Videos
                .Where(v => v.Id == dto.VideoId)
                .ExecuteUpdateAsync(v => v.SetProperty(x => x.CommentCount, x => x.CommentCount + 1),
                    cancellationToken);
        }

        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}