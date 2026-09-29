using Domain.Constants;
using FluentValidation;

namespace Application.Features.User.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.dto).NotNull().DependentRules(() =>
        {
            RuleFor(c => c.dto.Bio).MaximumLength(UserConstants.BioMaxLength)
                .WithErrorCode(ErrorCodes.TooLong);
        });
    }
}