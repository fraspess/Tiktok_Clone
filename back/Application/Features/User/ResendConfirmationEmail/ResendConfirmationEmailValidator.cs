using Domain.Constants;
using FluentValidation;

namespace Application.Features.User.ResendConfirmationEmail;

public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
    public ResendConfirmationEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ErrorCodes.EmailRequired)
            .EmailAddress().WithMessage(ErrorCodes.InvalidEmail);
    }
}