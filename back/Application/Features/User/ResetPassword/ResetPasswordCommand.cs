using MediatR;

namespace Application.Features.User.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Unit>;