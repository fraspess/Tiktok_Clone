using FluentValidation;

namespace Application.Features.User.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не може бути порожнім")
            .EmailAddress().WithMessage("Це не email");
    }
}