using Application.Dtos.Comment;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.Comment.Get
{
    public class GetCommentsQueryHandler(IUnitOfWork _uow, CommentMapper _mapper, ICurrentUser currentUser)
        : IRequestHandler<GetCommentsQuery, PagedResult<CommentDto>>
    {
        public async Task<PagedResult<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var comments = await _uow.Comments
                .GetCommentsByVideoId(request.VideoId)
                .ToProjectionDto(currentUser.Id)
                .ToPagedResultAsync(request.PaginationSettings);
            
            var result = comments.MapItems(_mapper.ToDto);
            return result;
        }
    }
}