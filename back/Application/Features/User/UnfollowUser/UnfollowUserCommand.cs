using MediatR;

namespace Application.Features.User.UnfollowUser;

public record UnfollowUserCommand(Guid FollowingId) : IRequest<Unit>;