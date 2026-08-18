using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.ChangeUsername;

internal class ChangeUsernameCommandHandler(
    UserManager<UserEntity> userManager,
    ICurrentUser currentUser,) : IRequestHandler<ChangeUsernameCommand, Unit>
{
    public async Task<Unit> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var id = currentUser.Id!.Value;

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
                   ?? throw new NotFoundException(ErrorCodes.UserNotFound);

        if (user.LastUsernameChangedAt.HasValue && DateTime.Now <= user.LastUsernameChangedAt.Value.AddDays(7))
            throw new BadRequestException(ErrorCodes.CooldownOnChangeUsername);

        if (await userManager.FindByNameAsync(request.newUsername) is not null)
            throw new BadRequestException(ErrorCodes.AlreadyExists);

        user.LastUsernameChangedAt = DateTime.UtcNow;
        var result = await userManager.SetUserNameAsync(user, request.newUsername);
        if (!result.Succeeded)
            throw new BadRequestException(ErrorCodes.InvalidUsername);



        return Unit.Value;
    }
}