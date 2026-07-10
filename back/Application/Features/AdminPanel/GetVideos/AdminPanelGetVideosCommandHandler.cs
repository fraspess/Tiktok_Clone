using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.GetVideos;

internal class AdminPanelGetVideosCommandHandler(IAppDbContext appDbContext, VideoMapper mapper, ICurrentUser user)
    : IRequestHandler<AdminPanelGetVideosCommand, PagedResult<SimpleVideoDto>>
{
    public async Task<PagedResult<SimpleVideoDto>> Handle(AdminPanelGetVideosCommand request,
        CancellationToken cancellationToken)
    {
        var videos = await appDbContext
            .Videos
            .IgnoreQueryFilters()
            .Where(v => v.Status == VideoStatus.Processed)
            .ToProjectionDto(user.Id)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken);

        var result = videos.MapItems(mapper.ToSimpleDto);
        return result;
    }
}