using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Update;

internal class UpdateUserCommandHandler(UserManager<UserEntity> userManager, IImageService imageService, ICurrentUser currentUser, IStorageService storageService) : IRequestHandler<UpdateUserCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var id = currentUser.Id!.Value;
        var dto = request.dto;
        
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new NotFoundException("Користувача не знайдено");

        user.Description = dto.Bio;

        if (dto.Avatar is not null)
        {
            await storageService.DeleteUserAvatars(user.Id);
            await imageService.SaveImageAsync(dto.Avatar, user.Id);
        }
        
        await userManager.UpdateAsync(user);
        return Unit.Value;
    }
}