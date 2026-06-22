using Application.Interfaces;
using MediatR;

namespace Application.Features.User.ResendConfirmationEmail;

public class ResendConfirmationEmailCommandHandler(IUserService service)
    : IRequestHandler<ResendConfirmationEmailCommand, Unit>
{
    public async Task<Unit> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        await service.ResendConfirmationEmailAsync(request.Email);
        return Unit.Value;
    }
}