using Application.Features.User.Login;
using Domain.Constants;
using Xunit;

namespace Application.Tests;

public class LoginValidationTests
{
    private readonly LoginUserCommandValidator _validator = new();
    [Fact]
    public void Login_WhenValidUsernameAndPassword()
    {
        var command = new LoginUserCommand("potuzhnesalo", "TestPotuzhnist1@");
        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Potuzhnicht!1")]
    [InlineData("s", "Potuzhnicht!1")]
    [InlineData("потужність", "Potuzhnicht!1")]
    public void Login_WhenInvalidUsernameAndValidPassword(string username, string password)
    {
        var command = new LoginUserCommand(username, password);
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "login");
    }
    
    [Fact]
    public void Login_WhenValidUsernameAndInvalidPassword()
    {
        var command = new LoginUserCommand("Papich", "");
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "password");
    }

    [Theory]
    [InlineData("Saa12aaa")]
    [InlineData("Sa!lbkkghbsd")]
    [InlineData("@12351515151")]
    public void Login_WhenValidUsernameAndWeakPassword(string password)
    {
        var command = new LoginUserCommand("potuzhnesalo", password);
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.ErrorCode == ErrorCodes.WeakPassword);
    }
    
}