using Application.Extensions;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.User.ChangeUsername;

public class ChangeUsernameCommandValidator : AbstractValidator<ChangeUsernameCommand>
{
    public ChangeUsernameCommandValidator()
    {
        RuleFor(c => c.newUsername).IsValidUsername().WithErrorCode(ErrorCodes.InvalidUsername);
    }
}