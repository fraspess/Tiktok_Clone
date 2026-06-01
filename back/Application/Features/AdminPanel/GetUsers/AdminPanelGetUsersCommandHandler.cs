using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Pagination;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.AdminPanel.GetUsers;

internal class AdminPanelGetUsersCommandHandler(UserManager<UserEntity> userManager, UserMapper mapper) : IRequestHandler<AdminPanelGetUsersCommand, PagedResult<SimpleUserDto>>
{
    public async Task<PagedResult<SimpleUserDto>> Handle(AdminPanelGetUsersCommand request, CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .ToProjectionDto(null)
            .ToPagedResultAsync(request.PaginationSettings);

        var result = users.MapItems(mapper.ToSimpleDto);
        return result;
    }
}