using Application.Dtos.Token;
using MediatR;

namespace Application.Features.User.RefreshTokens;

public record RefreshTokensCommand(string refreshToken) : IRequest<TokenResponseDTO>;