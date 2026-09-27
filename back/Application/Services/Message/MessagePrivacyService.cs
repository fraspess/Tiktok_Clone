using Application.Interfaces;
using Domain.Constants;
using Domain.Entities.Identity;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Message;

public class MessagePrivacyService(IAppDbContext db)
{
    public async Task EnsureCanMessageAsync(Guid senderId, Guid recipientId,
        CancellationToken cancellationToken = default)
    {
        var privacy = await db.Set<UserEntity>()
            .Where(u => u.Id == recipientId)
            .Select(u => (MessagePrivacy?)u.MessagePrivacy)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.UserNotFound);

        var allowed = privacy switch
        {
            MessagePrivacy.Everyone => true,
            MessagePrivacy.Followers => await db.UserFollows.AnyAsync(
                f => f.FollowerId == senderId && f.FollowingId == recipientId, cancellationToken),
            MessagePrivacy.Following => await db.UserFollows.AnyAsync(
                f => f.FollowerId == recipientId && f.FollowingId == senderId, cancellationToken),
            MessagePrivacy.MutualFollowers => await db.UserFollows.AnyAsync(
                f => f.FollowerId == senderId && f.FollowingId == recipientId, cancellationToken)
                && await db.UserFollows.AnyAsync(
                    f => f.FollowerId == recipientId && f.FollowingId == senderId, cancellationToken),
            _ => false,
        };

        if (!allowed) throw new NotAllowedException(ErrorCodes.MessagesNotAccepted);
    }
}
