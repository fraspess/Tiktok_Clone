using MediatR;

namespace Application.Features.User.ResendConfirmationEmail;

public record ResendConfirmationEmailCommand(string Email) : IRequest<Unit>;