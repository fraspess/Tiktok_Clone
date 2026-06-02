using Domain.Entities.Identity;
using Domain.Exceptions;
using Google.Apis.Logging;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AdminPanel.UnbanUser;

internal class UnbanUserCommandHandler(UserManager<UserEntity> userManager) : IRequestHandler<UnbanUserCommand, Unit>
{
    public async Task<Unit> Handle(UnbanUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
                   ?? throw new BadRequestException("Користувача не знайдено");

        user.IsBanned = false;
        user.BannedAt = null;
        user.BannedBy = null;
        user.BanReason = null;
        await userManager.UpdateAsync(user);
        return Unit.Value;
    }
}