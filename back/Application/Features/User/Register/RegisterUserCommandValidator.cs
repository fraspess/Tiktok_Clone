using Application.Extensions;
using Domain.Exceptions;
using FluentValidation;

namespace Application.Features.User.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ErrorCodes.EmailRequired)
            .EmailAddress().WithErrorCode(ErrorCodes.InvalidEmail)
            .MaximumLength(256).WithErrorCode(ErrorCodes.TooLong);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorCodes.PasswordRequired)
            .MinimumLength(6).WithErrorCode(ErrorCodes.TooShort)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$").WithErrorCode(ErrorCodes.WeakPassword);

        RuleFor(x => x.Username).IsValidUsername();
    }
}