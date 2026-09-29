using Application.Features.User.Register;
using Domain.Constants;
using Xunit;

namespace Application.Tests;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_HasNoErrors()
    {
        var result = _validator.Validate(new RegisterUserCommand(
            "SaloMaster", "salo@example.com", "StrongPassword1!"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", ErrorCodes.EmailRequired)]
    [InlineData("not-an-email", ErrorCodes.InvalidEmail)]
    public void Validate_WhenEmailIsInvalid_HasExpectedError(string email, string errorCode)
    {
        var result = _validator.Validate(new RegisterUserCommand(
            "SaloMaster", email, "StrongPassword1!"));

        Assert.Contains(result.Errors, error => error.PropertyName == "Email" && error.ErrorCode == errorCode);
    }

    [Theory]
    [InlineData("short", ErrorCodes.TooShort)]
    [InlineData("no-special-character1", ErrorCodes.WeakPassword)]
    public void Validate_WhenPasswordIsInvalid_HasExpectedError(string password, string errorCode)
    {
        var result = _validator.Validate(new RegisterUserCommand(
            "SaloMaster", "salo@example.com", password));

        Assert.Contains(result.Errors, error => error.PropertyName == "Password" && error.ErrorCode == errorCode);
    }
}
