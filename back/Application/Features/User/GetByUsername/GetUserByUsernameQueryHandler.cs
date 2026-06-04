using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.GetByUsername
{
    public class GetUserByUsernameQueryHandler(IUserService service, UserManager<UserEntity> userManager, UserMapper userMapper, ICurrentUser currentUser) : IRequestHandler<GetUserByUsernameQuery, UserDto>
    {
        public async Task<UserDto> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.Users.ToProjectionDto(currentUser.Id).FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken: cancellationToken)
                ?? throw new NotFoundException("Користувача не знайдено");
            return userMapper.ToDto(user);
        }
    }
}