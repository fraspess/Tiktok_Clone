using MediatR;

namespace Application.Features.User.FollowUser
{
    public record FollowUserCommand(Guid FollowingId) : IRequest<Unit>;
}