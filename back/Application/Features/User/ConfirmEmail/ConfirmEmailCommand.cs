using Application.Dtos.Token;
using MediatR;

namespace Application.Features.User.ConfirmEmail;

public record ConfirmEmailCommand(string Email, string Token) : IRequest<TokenResponseDTO>;