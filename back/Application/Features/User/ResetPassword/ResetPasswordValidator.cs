using FluentValidation;

namespace Application.Features.User.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Почта не може бути пустой")
            .EmailAddress().WithMessage("Невірний формат электронной почти");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Пароль не може бути пустим")
            .MinimumLength(6).WithMessage("Пароль має бути не менше ніж 6 символів");
    }
}