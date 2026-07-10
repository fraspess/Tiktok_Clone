using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.FollowUser;

public class FollowUserCommandHandler(
    UserManager<UserEntity> userManager,
    ICurrentUser currentUser,
    IAppDbContext appDbContext) : IRequestHandler<FollowUserCommand, Unit>
{
    public async Task<Unit> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        var followingUserExists = await userManager.Users.AnyAsync(u => u.Id == request.FollowingId, cancellationToken);
        if (!followingUserExists) throw new NotFoundException(ErrorCodes.UserNotFound);

        appDbContext.UserFollows.Add(new UserFollowEntity
        {
            FollowingId = request.FollowingId,
            FollowerId = currentUser.Id!.Value
        });
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

/*var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == follower)
       ?? throw new UnauthorizedException("Користувача не знайдено");
var isAlreadyFollw
var isAlreadyFollowing = await uow.Follows.GetFollowAsync(follower, following);

if (isAlreadyFollowing is null)
{
user.Following.Add(new UserFollowEntity { FollowerId = follower, FollowingId = following });
}
else
{
user.Following.Remove(isAlreadyFollowing);
}

await userManager.UpdateAsync(user);*/