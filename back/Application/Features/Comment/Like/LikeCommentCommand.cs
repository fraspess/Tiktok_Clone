using MediatR;

namespace Application.Features.Comment.Like;

public record LikeCommentCommand(Guid CommentId) : IRequest<Unit>;