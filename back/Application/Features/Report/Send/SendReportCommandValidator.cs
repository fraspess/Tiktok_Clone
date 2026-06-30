using Application.Dtos.Report;
using Domain;
using Domain.Exceptions;
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

                RuleFor(c => c.Dto)
                    .Must(dto => dto.Reason.HasValue || !string.IsNullOrWhiteSpace(dto.CustomReason))
                    .WithMessage("Необхідно вказати причину скарги");

                RuleFor(c => c.Dto)
                    .Must(dto => !dto.Reason.HasValue || dto.ContentType switch
                    {
                        ContentTypes.Video => Enum.IsDefined(typeof(VideoReportReasons), dto.Reason.Value),
                        ContentTypes.User => Enum.IsDefined(typeof(UserReportReasons), dto.Reason.Value),
                        ContentTypes.Comment => Enum.IsDefined(typeof(CommentReportReasons), dto.Reason.Value),
                        _ => false
                    })
                    .WithMessage("Невірна причина скарги");
            });
    }
}