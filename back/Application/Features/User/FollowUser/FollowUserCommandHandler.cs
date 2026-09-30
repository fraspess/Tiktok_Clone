using Application.Interfaces;
using Domain.Entities.Identity;
using Application.Services.Notification;
using Domain;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.FollowUser;

public class FollowUserCommandHandler(
    UserManager<UserEntity> userManager,
    ICurrentUser currentUser,
    IAppDbContext appDbContext,
    INotificationService notifications) : IRequestHandler<FollowUserCommand, Unit>
{
    public async Task<Unit> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        if (request.FollowingId == currentUser.Id) throw new NotAllowedException(ErrorCodes.Forbidden);

        var followingUserExists = await userManager.Users.AnyAsync(u => u.Id == request.FollowingId, cancellationToken);
        if (!followingUserExists) throw new NotFoundException(ErrorCodes.UserNotFound);

        var existingFollow = await appDbContext.UserFollows.FirstOrDefaultAsync(
            f => f.FollowerId == currentUser.Id && f.FollowingId == request.FollowingId,
            cancellationToken);

        if (existingFollow is not null) return Unit.Value;

        appDbContext.UserFollows.Add(new UserFollowEntity
        {
            FollowingId = request.FollowingId,
            FollowerId = currentUser.Id!.Value
        });

        var notification = notifications.Create(request.FollowingId, currentUser.Id!.Value, NotificationType.NewFollower);
        await appDbContext.SaveChangesAsync(cancellationToken);
        await notifications.PublishAsync([notification], cancellationToken);
        return Unit.Value;
    }
}
