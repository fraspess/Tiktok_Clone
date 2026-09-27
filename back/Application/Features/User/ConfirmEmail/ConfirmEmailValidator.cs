using Domain.Constants;
using FluentValidation;

namespace Application.Features.User.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ErrorCodes.EmailRequired)
            .EmailAddress().WithErrorCode(ErrorCodes.InvalidEmail);
    }
}