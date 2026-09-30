using Application.Dtos.Report;
using Domain;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Report.Send;

public class SendReportCommandValidator : AbstractValidator<SendReportCommand>
{
    public SendReportCommandValidator()
    {
        RuleFor(c => c.Dto)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(c => c.Dto.ContentId)
                    .NotEmpty().WithMessage("ContentId є обов'язковим");

                RuleFor(c => c.Dto.ContentType)
                    .IsInEnum().WithErrorCode(ErrorCodes.InvalidFileType);

                RuleFor(c => c.Dto.CustomReason)
                    .MaximumLength(255).WithErrorCode(ErrorCodes.TooLong)
                    .When(c => !string.IsNullOrWhiteSpace(c.Dto.CustomReason));

                RuleFor(c => c.Dto.Reason)
                    .MaximumLength(64).WithErrorCode(ErrorCodes.TooLong)
                    .When(c => !string.IsNullOrWhiteSpace(c.Dto.Reason));

                RuleFor(c => c.Dto)
                    .Must(dto => !string.IsNullOrWhiteSpace(dto.Reason) || !string.IsNullOrWhiteSpace(dto.CustomReason))
                    .WithMessage("Необхідно вказати причину скарги");

                RuleFor(c => c.Dto)
                    .Must(dto => dto.Reason is null || IsValidReason(dto.ContentType, dto.Reason))
                    .WithMessage("Невірна причина скарги");

                RuleFor(c => c.Dto)
                    .Must(dto => !string.Equals(dto.Reason, nameof(VideoReportReasons.Other), StringComparison.OrdinalIgnoreCase)
                                 || !string.IsNullOrWhiteSpace(dto.CustomReason))
                    .WithMessage("Для причини Other необхідно додати пояснення");

            });
        
        
    }

    private static bool IsValidReason(ContentTypes contentType, string reason)
    {
        return contentType switch
        {
            ContentTypes.Video =>
                Enum.GetNames<VideoReportReasons>()
                    .Contains(reason, StringComparer.OrdinalIgnoreCase),

            ContentTypes.User =>
                Enum.GetNames<UserReportReasons>()
                    .Contains(reason, StringComparer.OrdinalIgnoreCase),

            ContentTypes.Comment =>
                Enum.GetNames<CommentReportReasons>()
                    .Contains(reason, StringComparer.OrdinalIgnoreCase),

            _ => false
        };
    }
}
