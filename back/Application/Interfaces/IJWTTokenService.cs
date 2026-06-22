using Application.Dtos.Token;
using Application.Features.Video.Upload.CompleteUpload;
using Domain.Entities.Identity;

namespace Application.Interfaces;

public interface IJWTTokenService
{
    Task<TokenResponseDTO> GenerateTokensAsync(UserEntity user);

    Task<TokenResponseDTO> RefreshTokensAsync(string refreshToken);

    string GenerateUploadToken(Guid videoId, Guid userId);

    UploadTokenPayload ValidateUpdateToken(string token);
}