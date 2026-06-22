using MediatR;

namespace Application.Features.Comment.Delete;

public record DeleteCommentCommand(Guid CommentId) : IRequest<Unit>;