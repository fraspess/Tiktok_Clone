using Application.Dtos.Report;
using Application.Extensions;
using Application.Features.Report.Send;
using Domain;
using Xunit;

namespace Application.Tests;

public class SendReportCommandValidatorTests
{
    private readonly SendReportCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenVideoReportHasValidReason_HasNoErrors()
    {
        var result = _validator.Validate(new SendReportCommand(new ReportDTO
        {
            ContentId = "video-id",
            ContentType = ContentTypes.Video,
            Reason = nameof(VideoReportReasons.Spam)
        }));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenNoReasonIsProvided_HasErrors()
    {
        var result = _validator.Validate(new SendReportCommand(new ReportDTO
        {
            ContentId = "video-id",
            ContentType = ContentTypes.Video
        }));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenReasonIsNotDefinedForContentType_HasErrors()
    {
        var result = _validator.Validate(new SendReportCommand(new ReportDTO
        {
            ContentId = "user-id",
            ContentType = ContentTypes.User,
            Reason = "Fnnfffffffffffffffffff"
        }));

        Assert.False(result.IsValid);
    }
}
