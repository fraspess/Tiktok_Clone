using Application.Dtos.User;
using Application.Interfaces;
using MediatR;

namespace Application.Features.User.Register;

internal class RegisterUserCommandHandler(IUserService userService) : IRequestHandler<RegisterUserCommand, Unit>
{
    async Task<Unit> IRequestHandler<RegisterUserCommand, Unit>.Handle(RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        await userService.Register(new RegisterUserDto
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
        });
        return Unit.Value;
    }
}