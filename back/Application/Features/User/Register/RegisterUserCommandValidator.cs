using Application.Extensions;
using FluentValidation;

namespace Application.Features.User.Register
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email не може бути порожнім")
                .EmailAddress().WithMessage("Невірний формат email")
                .MaximumLength(256).WithMessage("Максимум 256 символів!");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль не може бути пустим")
                .MinimumLength(6).WithMessage("Пароль повинен містити не менше  ніж 6 символів");

            RuleFor(x => x.Username).IsValidUsername();
        }
    }
}