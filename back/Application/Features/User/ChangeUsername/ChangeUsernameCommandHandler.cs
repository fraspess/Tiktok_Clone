using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.ChangeUsername;

internal class ChangeUsernameCommandHandler(UserManager<UserEntity> userManager, ICurrentUser currentUser, ICacheService cache) : IRequestHandler<ChangeUsernameCommand, Unit>
{
    public async Task<Unit> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var id = currentUser.Id!.Value;

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken: cancellationToken) 
                   ?? throw new NotFoundException("Користувача не знайдено");
        
        var cacheKey = $"user:username:{user.UserName}";
        await cache.RemoveAsync(cacheKey);
        if (user.LastUsernameChangedAt.HasValue && (DateTime.Now <=  user.LastUsernameChangedAt.Value.AddDays(7)))
        {
            throw new BadRequestException("Ви можете змінювати ім'я користувача лише 1 раз в 7 днів");
        }

        if (await userManager.FindByNameAsync(request.newUsername) is not null)
            throw new BadRequestException("Ім'я користувача уже є зайнятим");
        
        user.UserName = request.newUsername;
        user.LastUsernameChangedAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        return Unit.Value;
    }
}