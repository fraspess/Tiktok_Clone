using Application.Dtos.Comment;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.Comment.Get;

public class GetCommentsQueryHandler(CommentMapper mapper, ICurrentUser currentUser, IAppDbContext dbContext)
    : IRequestHandler<GetCommentsQuery, PagedResult<CommentDto>>
{
    public async Task<PagedResult<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var videoId = await dbContext.Videos.GetIdFromShortIdAsync(request.VideoId, ct: cancellationToken);
        var comments = await dbContext
            .Comments
	    .Where(c => c.VideoId == videoId && c.ParentCommentId == null)
            .ToProjectionDto(currentUser.Id)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        var result = comments.MapItems(mapper.ToDto);
        return result;
    }
}