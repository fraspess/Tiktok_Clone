using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.Video.GetRepostedVideos;

internal class GetRepostedVideosCommandHandler(IAppDbContext appDbContext, ICurrentUser currentUser, VideoMapper videoMapper) : IRequestHandler<GetRepostedVideosCommand, PagedResult<SimpleVideoDto>>
{
    public async Task<PagedResult<SimpleVideoDto>> Handle(GetRepostedVideosCommand request, CancellationToken cancellationToken)
    {
        var repostedVideos = await appDbContext
            .Videos
            .Where(v => v.Reposts.Any(vr => vr.UserId == request.UserId))
            .ToProjectionDto(currentUser.Id)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken);

        var mapped = repostedVideos.MapItems(videoMapper.ToSimpleDto);
        return mapped;
    }
}