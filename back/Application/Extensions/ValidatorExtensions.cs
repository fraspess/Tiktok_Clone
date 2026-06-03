using Domain.Constants;
using FluentValidation;

namespace Application.Extensions;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> IsValidUsername<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(UserConstants.UsernameMinLength)
            .MaximumLength(UserConstants.UsernameMaxLength)
            .Matches(UserConstants.UsernameRegex)
            .WithMessage(UserConstants.UsernameRegexMessage);
    }
}