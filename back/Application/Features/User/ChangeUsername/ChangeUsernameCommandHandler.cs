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
    ICurrentUser currentUser,
    ICacheService cache) : IRequestHandler<ChangeUsernameCommand, Unit>
{
    public async Task<Unit> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var id = currentUser.Id!.Value;

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
                   ?? throw new NotFoundException(ErrorCodes.UserNotFound);

        var cacheKey = $"user:username:{user.UserName}";
        await cache.RemoveAsync(cacheKey);
        if (user.LastUsernameChangedAt.HasValue && DateTime.Now <= user.LastUsernameChangedAt.Value.AddDays(7))
            throw new BadRequestException(ErrorCodes.CooldownOnChangeUsername);

        if (await userManager.FindByNameAsync(request.newUsername) is not null)
            throw new BadRequestException(ErrorCodes.AlreadyExists);

        // IMPORTANT: setting user.UserName directly and calling UpdateAsync does NOT
        // update NormalizedUserName in ASP.NET Core Identity. Since lookups (login,
        // FindByNameAsync, etc.) are done against NormalizedUserName, the old username
        // would keep working and the new one wouldn't be found until the user was
        // reloaded from a source that re-normalizes it. SetUserNameAsync updates both
        // UserName and NormalizedUserName and persists the change.
        user.LastUsernameChangedAt = DateTime.UtcNow;
        var result = await userManager.SetUserNameAsync(user, request.newUsername);
        if (!result.Succeeded)
            throw new BadRequestException(ErrorCodes.InvalidUsername);

        return Unit.Value;
    }
}