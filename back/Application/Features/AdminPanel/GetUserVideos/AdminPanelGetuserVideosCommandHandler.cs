using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.GetUserVideos;

internal class AdminPanelGetuserVideosCommandHandler(IAppDbContext appDbContext, VideoMapper mapper, ICurrentUser currentUser) : IRequestHandler<AdminPanelGetUserVideosCommand, PagedResult<SimpleVideoDto>>
{
    public async Task<PagedResult<SimpleVideoDto>> Handle(AdminPanelGetUserVideosCommand request, CancellationToken cancellationToken)
    {
        var videos = await appDbContext
            .Videos
            .IgnoreQueryFilters()
            .Where(v => v.UserId == request.UserId && v.Status == VideoStatus.Processed)
            .ToProjectionDto(currentUser.Id)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        var result = videos.MapItems(mapper.ToSimpleDto);
        return result;
    }
}