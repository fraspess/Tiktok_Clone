using Application.Dtos.Comment;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.Comment.GetReplies
{
    public class GetRepliesQueryHandler(IUnitOfWork _uow, CommentMapper _mapper, ICurrentUser currentUser)
        : IRequestHandler<GetRepliesQuery, PagedResult<CommentDto>>
    {
        public async Task<PagedResult<CommentDto>> Handle(GetRepliesQuery request, CancellationToken cancellationToken)
        {
            var replies = await _uow.Comments
                .GetRepliesAsync(request.ParentCommentId)
                .ToProjectionDto(currentUser.Id)
                .ToPagedResultAsync(request.PaginationSettings);


            var result = replies.MapItems(_mapper.ToDto);
            return result;
        }
    }
}