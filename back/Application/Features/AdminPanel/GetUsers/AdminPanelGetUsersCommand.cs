using Application.Dtos.User;
using Application.Pagination;
using MediatR;

namespace Application.Features.AdminPanel.GetUsers;

public record AdminPanelGetUsersCommand(PaginationSettings PaginationSettings) : IRequest<PagedResult<SimpleUserDto>>;