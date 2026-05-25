using Application.Interfaces;
using Domain.Entities.Comment;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Comment.Create
{
    public class CreateCommendCommandHandler(IUnitOfWork _uow, ICurrentUser currentUser) : IRequestHandler<CreateCommentCommand, Unit>
    {
        public async Task<Unit> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var ownerId = currentUser.Id!.Value;
            if (dto.ParentCommentId is not null)
            {
                var comment = await _uow.Comments.GetByIdAsync(dto.ParentCommentId.Value)
                              ?? throw new ValidationException("Коментарій не знайдено");
                var newComment = new CommentEntity
                {
                    Text = dto.Text, ParentCommentId = dto.ParentCommentId.Value, UserId = ownerId,
                    VideoId = dto.VideoId
                };
                await _uow.Comments.CreateAsync(newComment);
            }
            else
            {
                var comment = new CommentEntity { Text = dto.Text, UserId = ownerId, VideoId = dto.VideoId };
                await _uow.Comments.CreateAsync(comment);
            }

            await _uow.SaveChangesAsync();
            return Unit.Value;
        }
    }
}