using Application.Dtos.Comment;
using Application.Pagination;
using MediatR;

namespace Application.Features.Comment.Get
{
    public record GetCommentsQuery(Guid VideoId, PaginationSettings PaginationSettings)
        : IRequest<PagedResult<CommentDto>>;
}