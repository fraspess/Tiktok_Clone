using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.BanUser;

internal class BanUserCommandHandler(UserManager<UserEntity> userManager, ICurrentUser currentUser) : IRequestHandler<BanUserCommand, Unit>
{
    public async Task<Unit> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Користувача не знайдено");
        
        user.BannedBy = currentUser.Id;
        user.BannedAt = DateTime.UtcNow;
        user.BanReason = request.Reason;
        user.IsBanned = true;
        await userManager.UpdateAsync(user);
        return Unit.Value;
    }
}