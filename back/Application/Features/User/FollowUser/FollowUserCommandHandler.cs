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
    ICacheService cache,
    IAppDbContext appDbContext) : IRequestHandler<FollowUserCommand, Unit>
{
    public async Task<Unit> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        var followerId = currentUser.Id!.Value;

        if (request.FollowingId == followerId)
            throw new BadRequestException(ErrorCodes.ValidationError);

        var followingUsername = await userManager.Users
            .Where(u => u.Id == request.FollowingId)
            .Select(u => u.UserName)
            .FirstOrDefaultAsync(cancellationToken);
        if (followingUsername is null) throw new NotFoundException(ErrorCodes.UserNotFound);

        var existingFollow = await appDbContext.UserFollows.FirstOrDefaultAsync(
            f => f.FollowerId == followerId && f.FollowingId == request.FollowingId, cancellationToken);

        if (existingFollow is null)
        {
            appDbContext.UserFollows.Add(new UserFollowEntity
            {
                FollowingId = request.FollowingId,
                FollowerId = followerId
            });
        }
        else
        {
            appDbContext.UserFollows.Remove(existingFollow);
        }

        await appDbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync($"user:username:{followingUsername}:{followerId}");
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