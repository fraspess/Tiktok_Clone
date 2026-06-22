using Application.Dtos.Token;
using Application.Interfaces;
using MediatR;

namespace Application.Features.User.GoogleAuth;

public class GoogleAuthCommandHandler(IUserService service) : IRequestHandler<GoogleAuthCommand, TokenResponseDTO>
{
    public async Task<TokenResponseDTO> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        return await service.GoogleAuth(request.IdToken);
    }
}