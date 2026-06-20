using FluentValidation;

namespace Application.Features.User.ResendConfirmationEmail;

public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
    public ResendConfirmationEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Почта не може бути пустою")
            .EmailAddress().WithMessage("Це не email");
    }
}