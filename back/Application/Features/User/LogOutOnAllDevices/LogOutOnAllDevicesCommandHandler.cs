using Application.Interfaces;
using MediatR;

namespace Application.Features.User.LogOutOnAllDevices;

public class LogOutOnAllDevicesCommandHandler(IUserService userService, ICurrentUser user)
    : IRequestHandler<LogOutOnAllDevicesCommand, Unit>
{
    public async Task<Unit> Handle(LogOutOnAllDevicesCommand request, CancellationToken cancellationToken)
    {
        await userService.UpdateTokenVersion(user.Id!.Value);
        return Unit.Value;
    }
}