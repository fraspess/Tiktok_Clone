using Application.Interfaces;
using MediatR;

namespace Application.Features.User.ForgotPassword;

public class ForgotPasswordCommandHandler(IUserService userService) : IRequestHandler<ForgotPasswordCommand, Unit>
{
    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        await userService.ForgotPasswordAsync(request.email);
        return Unit.Value;
    }
}