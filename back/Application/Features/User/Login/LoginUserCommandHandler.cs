using Application.Dtos.Token;
using Application.Dtos.User;
using Application.Interfaces;
using MediatR;

namespace Application.Features.User.Login;

public class LoginUserCommandHandler(IUserService userService)
    : IRequestHandler<LoginUserCommand, TokenResponseDTO>
{
    public async Task<TokenResponseDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await userService.Login(new LoginUserDto
        {
            Login = request.login,
            Password = request.password
        });
    }
}