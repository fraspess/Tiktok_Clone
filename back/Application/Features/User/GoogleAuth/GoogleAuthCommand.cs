using Application.Dtos.Token;
using MediatR;

namespace Application.Features.User.GoogleAuth;

public record GoogleAuthCommand(string IdToken) : IRequest<TokenResponseDTO>;