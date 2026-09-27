using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.UnfollowUser;

public class UnfollowUserCommandHandler(
    ICurrentUser currentUser,
    IAppDbContext appDbContext) : IRequestHandler<UnfollowUserCommand, Unit>
{
    public async Task<Unit> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var existingFollow = await appDbContext.UserFollows.FirstOrDefaultAsync(
            f => f.FollowerId == currentUser.Id && f.FollowingId == request.FollowingId,
            cancellationToken);

        if (existingFollow is null) return Unit.Value;

        appDbContext.UserFollows.Remove(existingFollow);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}