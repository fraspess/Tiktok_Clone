using Application.Interfaces;
using MediatR;

namespace Application.Features.User.FollowUser
{
    public class FollowUserCommandHandler(IUserService service, ICurrentUser user) : IRequestHandler<FollowUserCommand, Unit>
    {
        public async Task<Unit> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            await service.ToggleFollowAsync(user.Id!.Value, request.FollowingId);
            return Unit.Value;
        }
    }
}