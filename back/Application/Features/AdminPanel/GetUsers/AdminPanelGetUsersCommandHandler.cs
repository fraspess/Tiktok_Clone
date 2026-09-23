using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.AdminPanel.GetUsers;

internal class AdminPanelGetUsersCommandHandler(UserManager<UserEntity> userManager, UserMapper mapper)
    : IRequestHandler<AdminPanelGetUsersCommand, PagedResult<SimpleUserDto>>
{
    public async Task<PagedResult<SimpleUserDto>> Handle(AdminPanelGetUsersCommand request,
        CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();
        var usersQuery = userManager.Users.Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.ToUpperInvariant();
            usersQuery = usersQuery.Where(u => u.UserName != null && u.UserName.ToUpper().Contains(normalizedSearch));
        }

        if (request.IsBanned.HasValue)
        {
            usersQuery = usersQuery.Where(u => u.IsBanned == request.IsBanned.Value);
        }

        var users = await usersQuery
            .OrderBy(u => u.UserName)
            .ToProjectionDto(null)
            .ToPagedResultAsync(request.PaginationSettings, cancellationToken: cancellationToken);

        var result = users.MapItems(mapper.ToSimpleDto);
        return result;
    }
}
