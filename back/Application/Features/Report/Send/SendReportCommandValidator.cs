using Application.Dtos.Report;
using Domain;
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
                RuleFor(c => c.Dto.VideoReportReason)
                    .IsInEnum().WithMessage("Невірний формат причини для скарги")
                    .When(c => c.Dto.ContentType == ContentTypes.Video 
                               && c.Dto.VideoReportReason.HasValue);

                RuleFor(c => c.Dto.CommentReportReason)
                    .IsInEnum().WithMessage("Невірний формат причини для скарги")
                    .When(c => c.Dto.ContentType == ContentTypes.Comment 
                               && c.Dto.CommentReportReason.HasValue);

                RuleFor(c => c.Dto.UserReportReason)
                    .IsInEnum().WithMessage("Невірний формат причини для скарги")
                    .When(c => c.Dto.ContentType == ContentTypes.User 
                               && c.Dto.UserReportReason.HasValue);
                
                RuleFor(c => c.Dto.CustomReason)
                    .MaximumLength(255).WithMessage("Інша причина не може перевищувати 255 символів")
                    .When(c => !string.IsNullOrWhiteSpace(c.Dto.CustomReason));
                
                RuleFor(c => c.Dto)
                    .Must(HasAtLeastOneReason)
                    .WithMessage("Потрібно вказати хоч одну причину для скарги");
                
                RuleFor(c => c.Dto)
                    .Must(dto => !(HasEnumReason(dto) && !string.IsNullOrWhiteSpace(dto.CustomReason)))
                    .WithMessage("Можна вказати лише одну причину скарги");
            });
        
    }
        private static bool HasEnumReason(ReportDTO dto) => dto.ContentType switch
        {
            ContentTypes.Video   => dto.VideoReportReason.HasValue,
            ContentTypes.Comment => dto.CommentReportReason.HasValue,
            ContentTypes.User    => dto.UserReportReason.HasValue,
            _                    => false
        };

        private static bool HasAtLeastOneReason(ReportDTO dto)
            => HasEnumReason(dto) || !string.IsNullOrWhiteSpace(dto.CustomReason);
}