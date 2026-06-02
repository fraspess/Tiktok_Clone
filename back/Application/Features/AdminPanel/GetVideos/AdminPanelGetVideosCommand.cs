using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.AdminPanel.GetVideos;

public record AdminPanelGetVideosCommand(PaginationSettings PaginationSettings) : IRequest<PagedResult<SimpleVideoDto>>;