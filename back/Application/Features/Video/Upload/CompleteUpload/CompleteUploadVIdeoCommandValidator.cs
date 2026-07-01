using Domain.Exceptions;
using FluentValidation;

namespace Application.Features.Video.Upload.CompleteUpload;

public class CompleteUploadVideoCommandValidator : AbstractValidator<CompleteUploadVideoCommand>
{
    public CompleteUploadVideoCommandValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(500).WithErrorCode(ErrorCodes.TooLong);
    }
}