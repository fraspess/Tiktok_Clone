using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.AdminPanel.GetUserVideos;

internal class AdminPanelGetuserVideosCommandHandler(IUnitOfWork _uow, VideoMapper mapper, ICurrentUser currentUser) : IRequestHandler<AdminPanelGetUserVideosCommand, PagedResult<SimpleVideoDto>>
{
    public async Task<PagedResult<SimpleVideoDto>> Handle(AdminPanelGetUserVideosCommand request, CancellationToken cancellationToken)
    {
        var videos = await _uow.Videos
            .GetAllIgnoreQueryFilters()
            .Where(v => v.UserId == request.UserId)
            .ToProjectionDto(currentUser.Id)
            .ToPagedResultAsync(request.PaginationSettings);

        var result = videos.MapItems(mapper.ToSimpleDto);
        return result;
    }
}