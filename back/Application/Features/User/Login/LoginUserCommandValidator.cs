using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Application.Extensions;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.User.Login;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.login)
            .NotEmpty().WithErrorCode(ErrorCodes.Required)
            .Must(BeValidUsernameOrEmail).WithErrorCode(ErrorCodes.InvalidCredentials);

        RuleFor(x => x.password)
            .NotEmpty().WithErrorCode(ErrorCodes.PasswordRequired)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$").WithErrorCode(ErrorCodes.WeakPassword);
    }

    private static bool BeValidUsernameOrEmail(string login)
    {
        var isUsername =
            login.Length >= UserConstants.UsernameMinLength &&
            login.Length <= UserConstants.UsernameMaxLength &&
            Regex.IsMatch(login, UserConstants.UsernameRegex);

        var isEmail = new EmailAddressAttribute().IsValid(login);

        return isUsername || isEmail;
    }
}