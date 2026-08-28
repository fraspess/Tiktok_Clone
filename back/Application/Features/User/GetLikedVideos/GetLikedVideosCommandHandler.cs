using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.User.GetLikedVideos;

public class GetLikedVideosCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser, VideoMapper videoMapper) : IRequestHandler<GetLikedVideosCommand, PagedResult<SimpleVideoDto>>
{
    public async Task<PagedResult<SimpleVideoDto>> Handle(GetLikedVideosCommand request, CancellationToken cancellationToken)
    {
        var likedVideos = await appDbContext.Videos
            .Where(v => v.Likes.Any(l => l.UserId == request.UserId))
            .ToProjectionDto(currentUser.Id)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        var mapped = likedVideos.MapItems(videoMapper.ToSimpleDto);
        return mapped;
    }
}