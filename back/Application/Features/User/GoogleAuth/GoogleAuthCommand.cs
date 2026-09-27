using Application.Dtos.Token;
using MediatR;

namespace Application.Features.User.GoogleAuth;

public record GoogleAuthCommand(string Code) : IRequest<TokenResponseDTO>;