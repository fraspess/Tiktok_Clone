using Application.Features.Video.Upload.CompleteUpload;
using Domain.Constants;
using Xunit;

namespace Application.Tests;

public class CompleteUploadVideoCommandValidatorTests
{
    private readonly CompleteUploadVideoCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenDescriptionIsExactlyFourThousandCharacters_HasNoErrors()
    {
        var result = _validator.Validate(new CompleteUploadVideoCommand("token", new string('a', 4000)));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenDescriptionExceedsFourThousandCharacters_HasTooLongError()
    {
        var result = _validator.Validate(new CompleteUploadVideoCommand("token", new string('a', 4001)));

        Assert.Contains(result.Errors, error => error.PropertyName == "Description" && error.ErrorCode == ErrorCodes.TooLong);
    }
}
