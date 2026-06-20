using MediatR;

namespace Application.Features.AdminPanel.DeleteComment;

public record DeleteCommentCommand(Guid Id) : IRequest<Unit>;