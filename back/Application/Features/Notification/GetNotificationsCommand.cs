using Application.Dtos.Notification;
using Application.Pagination;
using MediatR;

namespace Application.Features.Notification;

public record GetNotificationsCommand(PaginationSettings PaginationSettings) : IRequest<PagedResult<NotificationDto>>;