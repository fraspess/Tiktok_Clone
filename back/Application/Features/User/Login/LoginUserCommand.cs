using Application.Dtos.Token;
using MediatR;

namespace Application.Features.User.Login;

public record LoginUserCommand(string login, string password) : IRequest<TokenResponseDTO>;