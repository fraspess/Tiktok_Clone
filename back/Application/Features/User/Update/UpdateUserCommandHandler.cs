using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Update;

internal class UpdateUserCommandHandler(UserManager<UserEntity> userManager, IImageService imageService, ICurrentUser currentUser) : IRequestHandler<UpdateUserCommand, Unit>
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
            imageService.DeleteImage(user.Avatar!);
            var newImage = await imageService.SaveImageAsync(dto.Avatar);
            user.Avatar = newImage;
        }
        
        await userManager.UpdateAsync(user);
        return Unit.Value;
    }
}