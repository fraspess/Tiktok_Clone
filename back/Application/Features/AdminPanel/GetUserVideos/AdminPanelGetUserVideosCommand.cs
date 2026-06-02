using Application.Dtos.Video;
using Application.Pagination;
using MediatR;

namespace Application.Features.AdminPanel.GetUserVideos;

public record AdminPanelGetUserVideosCommand (Guid UserId, PaginationSettings PaginationSettings): IRequest<PagedResult<SimpleVideoDto>>;