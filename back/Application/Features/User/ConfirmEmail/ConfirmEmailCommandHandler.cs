using Application.Dtos.Token;
using Application.Interfaces;
using MediatR;

namespace Application.Features.User.ConfirmEmail;

public class ConfirmEmailCommandHandler(IUserService userService)
    : IRequestHandler<ConfirmEmailCommand, TokenResponseDTO>
{
    public async Task<TokenResponseDTO> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        return await userService.ConfirmEmail(request.Email, request.Token);
    }
}