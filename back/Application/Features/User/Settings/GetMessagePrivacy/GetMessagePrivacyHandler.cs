using Application.Interfaces;
using Domain.Constants;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Settings.GetMessagePrivacy;

internal class GetMessagePrivacyHandler(ICurrentUser currentUser, UserManager<UserEntity> userManager) : IRequestHandler<GetMessagePrivacyCommand, MessagePrivacy>
{
    public async Task<MessagePrivacy> Handle(GetMessagePrivacyCommand request, CancellationToken cancellationToken)
    {
        var userPrivacy = await userManager.Users
            .Where(u => u.Id == currentUser.Id)
            .Select(u => u.MessagePrivacy)
            .FirstOrDefaultAsync(cancellationToken);

        return userPrivacy;
    }
}