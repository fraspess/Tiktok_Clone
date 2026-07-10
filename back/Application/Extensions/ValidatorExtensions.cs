using Domain.Constants;
using FluentValidation;

namespace Application.Extensions;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> IsValidUsername<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithErrorCode(ErrorCodes.UsernameRequired)
            .MinimumLength(UserConstants.UsernameMinLength).WithErrorCode(ErrorCodes.TooShort)
            .MaximumLength(UserConstants.UsernameMaxLength).WithErrorCode(ErrorCodes.TooLong)
            .Matches(UserConstants.UsernameRegex).WithErrorCode(ErrorCodes.InvalidUsername);
    }
}