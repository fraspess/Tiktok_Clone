using Application.Extensions;
using FluentValidation;

namespace Application.Features.User.ChangeUsername;

public class ChangeUsernameCommandValidator : AbstractValidator<ChangeUsernameCommand>
{
    public ChangeUsernameCommandValidator()
    {
        RuleFor(c => c.newUsername).IsValidUsername();
    }
}