using Domain.Constants;
using FluentValidation;

namespace Application.Features.User.ForgotPassword;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.email)
            .NotEmpty().WithErrorCode(ErrorCodes.EmailRequired)
            .EmailAddress().WithErrorCode(ErrorCodes.InvalidEmail);
    }
}