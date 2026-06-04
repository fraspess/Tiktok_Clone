using Application.Interfaces;
using Domain.Entities.Comment;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Comment.Create
{
    public class CreateCommendCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser) : IRequestHandler<CreateCommentCommand, Unit>
    {
        public async Task<Unit> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var video = await appDbContext.Videos.FirstOrDefaultAsync(v => v.Id == dto.VideoId, cancellationToken: cancellationToken) 
                        ?? throw new NotFoundException("Відео не знайдено");
            
            var ownerId = currentUser.Id!.Value;
            if (dto.ParentCommentId is not null)
            {
                var comment = await appDbContext.Comments.FirstOrDefaultAsync(c => c.Id == dto.ParentCommentId, cancellationToken: cancellationToken)
                              ?? throw new ValidationException("Коментарій не знайдено");
                var newComment = new CommentEntity
                {
                    Text = dto.Text, ParentCommentId = dto.ParentCommentId.Value, UserId = ownerId,
                    VideoId = dto.VideoId
                };
                await appDbContext.Comments.AddAsync(newComment, cancellationToken);
            }
            else
            {
                var comment = new CommentEntity { Text = dto.Text, UserId = ownerId, VideoId = dto.VideoId };
                await appDbContext.Comments.AddAsync(comment, cancellationToken);
            }

            await appDbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}