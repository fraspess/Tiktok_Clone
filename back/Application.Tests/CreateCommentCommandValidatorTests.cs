using Application.Dtos.Comment;
using Application.Features.Comment.Create;
using Domain.Constants;
using Xunit;

namespace Application.Tests;

public class CreateCommentCommandValidatorTests
{
    private readonly CreateCommentCommandValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenTextIsEmpty_HasRequiredError(string text)
    {
        var result = _validator.Validate(new CreateCommentCommand(new CreateCommentDto { Text = text }));

        Assert.Contains(result.Errors, error => error.PropertyName == "Dto.Text" && error.ErrorCode == ErrorCodes.Required);
    }

    [Fact]
    public void Validate_WhenTextExceedsFiveHundredCharacters_HasTooLongError()
    {
        var result = _validator.Validate(new CreateCommentCommand(new CreateCommentDto
        {
            Text = new string('a', 501)
        }));

        Assert.Contains(result.Errors, error => error.PropertyName == "Dto.Text" && error.ErrorCode == ErrorCodes.TooLong);
    }
}
