using Application.Dtos.Comment;
using Application.Pagination;
using MediatR;

namespace Application.Features.Comment.GetReplies;

public record GetRepliesQuery(Guid ParentCommentId, PaginationSettings PaginationSettings)
    : IRequest<PagedResult<CommentDto>>;