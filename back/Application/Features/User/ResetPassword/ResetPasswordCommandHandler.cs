using Application.Dtos.User;
using Application.Interfaces;
using MediatR;

namespace Application.Features.User.ResetPassword
{
    public class ResetPasswordCommandHandler(IUserService userService) : IRequestHandler<ResetPasswordCommand, Unit>
    {
        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            await userService.ResetPasswordAsync(new ResetPasswordDto
            {
                Email = request.Email,
                NewPassword = request.NewPassword,
                Token = request.Token,
            });
            return Unit.Value;
        }
    }
}