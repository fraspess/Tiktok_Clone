using Application.Dtos.Video;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using MediatR;

namespace Application.Features.AdminPanel.GetVideos;

internal class AdminPanelGetVideosCommandHandler(IUnitOfWork _uow, VideoMapper mapper, ICurrentUser user) : IRequestHandler<AdminPanelGetVideosCommand, PagedResult<SimpleVideoDto>>
{
    public async Task<PagedResult<SimpleVideoDto>> Handle(AdminPanelGetVideosCommand request, CancellationToken cancellationToken)
    {
        var videos = await _uow.Videos.GetAllIgnoreQueryFilters()
            .ToProjectionDto(user.Id)
            .ToPagedResultAsync(request.PaginationSettings);
        
        var result = videos.MapItems(mapper.ToSimpleDto);
        return result;
    }
}