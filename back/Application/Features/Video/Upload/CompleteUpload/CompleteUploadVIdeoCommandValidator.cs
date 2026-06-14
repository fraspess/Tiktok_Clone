using FluentValidation;

namespace Application.Features.Video.Upload.CompleteUpload;

public class CompleteUploadVideoCommandValidator : AbstractValidator<CompleteUploadVideoCommand>
{
    public CompleteUploadVideoCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotNull().WithMessage("Опис не може бути порожнім")
            .NotEmpty().WithMessage("Опис відео не може бути порожнім")
            .MaximumLength(500);
    }
}