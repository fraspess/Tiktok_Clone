using Application.Dtos.Token;
using Domain.Entities.Identity;

namespace Application.Interfaces;

public interface IJWTTokenService
{
    Task<TokenResponseDTO> GenerateTokensAsync(UserEntity user);

    Task<TokenResponseDTO> RefreshTokensAsync(string refreshToken);
}