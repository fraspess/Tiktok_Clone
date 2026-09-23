using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.AdminPanel.GetVideos;

public record AdminPanelGetVideosCommand(PaginationSettings PaginationSettings, bool? IsBanned) : IRequest<PagedResult<SimpleVideoDto>>;
