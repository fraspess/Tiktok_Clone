using Application.Dtos.Token;
using Application.Interfaces;
using MediatR;

namespace Application.Features.User.RefreshTokens;

public class RefreshTokensCommandHandler(IJwtTokenService jWtTokenService)
    : IRequestHandler<RefreshTokensCommand, TokenResponseDTO>
{
    public async Task<TokenResponseDTO> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        return await jWtTokenService.RefreshTokensAsync(request.refreshToken);
    }
}