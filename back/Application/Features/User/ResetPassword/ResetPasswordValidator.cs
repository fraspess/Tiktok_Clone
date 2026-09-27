using Domain.Constants;
using FluentValidation;

namespace Application.Features.User.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ErrorCodes.EmailRequired)
            .EmailAddress().WithErrorCode(ErrorCodes.InvalidEmail);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithErrorCode(ErrorCodes.PasswordRequired)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$").WithErrorCode(ErrorCodes.InvalidPassword);
    }
}