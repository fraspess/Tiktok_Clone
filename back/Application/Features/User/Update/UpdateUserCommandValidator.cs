using FluentValidation;

namespace Application.Features.User.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.dto).NotNull().DependentRules(() =>
        {
            RuleFor(c => c.dto.Bio).MaximumLength(160)
                .WithMessage("Опис профілю не може містити більше ніж 160 символів");
        });
    }
}