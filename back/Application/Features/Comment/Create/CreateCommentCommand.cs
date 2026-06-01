using Application.Dtos.Comment;
using MediatR;

namespace Application.Features.Comment.Create
{
    public record CreateCommentCommand(CreateCommentDto Dto) : IRequest<Unit>;
}