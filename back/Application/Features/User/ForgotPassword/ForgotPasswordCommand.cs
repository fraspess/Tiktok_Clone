using MediatR;

namespace Application.Features.User.ForgotPassword;

public record ForgotPasswordCommand(string email) : IRequest<Unit>;