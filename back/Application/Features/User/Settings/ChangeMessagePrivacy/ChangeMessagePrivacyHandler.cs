using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Settings.ChangeMessagePrivacy;

internal class ChangeMessagePrivacyHandler(ICurrentUser currentUser, UserManager<UserEntity> userManager) : IRequestHandler<ChangeMessagePrivacyCommand, Unit>
{
    public async Task<Unit> Handle(ChangeMessagePrivacyCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.newMessagePrivacy))
            throw new BadRequestException(ErrorCodes.InvalidValue);

        await userManager.Users
            .Where(u => u.Id == currentUser.Id)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.MessagePrivacy,request.newMessagePrivacy), cancellationToken: cancellationToken);
        return Unit.Value;
    }
}
