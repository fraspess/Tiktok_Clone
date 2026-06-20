using Application.Dtos.User;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.GetUserById;

internal class GetUserByIdCommandHandler(
    UserManager<UserEntity> userManager,
    UserMapper mapper,
    IStorageService storageService) : IRequestHandler<GetUserByIdCommand, GetUserAdminDto>
{
    public async Task<GetUserAdminDto> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
                   ?? throw new NotFoundException("Користувача не знайдено");
        var dto = mapper.ToGetUserAdminDto(user);
        dto.Avatar = storageService.GetUserAvatar(user.Id);
        return dto;
    }
}