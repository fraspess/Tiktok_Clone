using Domain.Exceptions;
using FluentValidation;

namespace Application.Features.User.Login;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.login)
            .NotEmpty().WithErrorCode(ErrorCodes.Required);

        RuleFor(x => x.password)
            .NotEmpty().WithErrorCode(ErrorCodes.PasswordRequired)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$").WithErrorCode(ErrorCodes.WeakPassword);
    }
}