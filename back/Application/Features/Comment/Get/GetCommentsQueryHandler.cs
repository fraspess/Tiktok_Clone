using Application.Dtos.Comment;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.Comment.Get;

public class GetCommentsQueryHandler(CommentMapper _mapper, ICurrentUser currentUser, IAppDbContext dbContext)
    : IRequestHandler<GetCommentsQuery, PagedResult<CommentDto>>
{
    public async Task<PagedResult<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = await dbContext
            .Comments
            .Where(c => c.VideoId == request.VideoId)
            .ToProjectionDto(currentUser.Id)
            .ToPagedResultAsync(request.PaginationSettings);

        var result = comments.MapItems(_mapper.ToDto);
        return result;
    }
}