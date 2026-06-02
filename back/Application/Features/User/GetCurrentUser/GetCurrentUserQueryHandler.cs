using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.GetCurrentUser
{
    public class GetCurrentUserQueryHandler(UserManager<UserEntity> userManager, UserMapper mapper, IStorageService storageService, ICurrentUser currentUser)
        : IRequestHandler<GetCurrentUserQuery, UserMeDto>
    {
        public async Task<UserMeDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.Users
                    .Where(u => u.Id == currentUser.Id)
                    .ToProjectionDto(currentUser.Id)
                    .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException("Користувача не знайдено?");
            
            return mapper.ToMeDto(user);
        }
    }
}