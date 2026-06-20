using FluentValidation;

namespace Application.Features.User.ForgotPassword;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.email)
            .NotEmpty().WithMessage("Email не може бути порожнім")
            .EmailAddress().WithMessage("Невірний формат email");
    }
}