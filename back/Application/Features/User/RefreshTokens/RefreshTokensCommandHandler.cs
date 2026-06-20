using Application.Dtos.Token;
using Application.Interfaces;
using MediatR;

namespace Application.Features.User.RefreshTokens;

public class RefreshTokensCommandHandler(IJWTTokenService jWTTokenService)
    : IRequestHandler<RefreshTokensCommand, TokenResponseDTO>
{
    public async Task<TokenResponseDTO> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        return await jWTTokenService.RefreshTokensAsync(request.refreshToken);
    }
}