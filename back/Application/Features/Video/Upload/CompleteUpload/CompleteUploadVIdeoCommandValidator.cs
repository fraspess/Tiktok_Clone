using Domain.Constants;
using FluentValidation;

namespace Application.Features.Video.Upload.CompleteUpload;

public class CompleteUploadVideoCommandValidator : AbstractValidator<CompleteUploadVideoCommand>
{
    public CompleteUploadVideoCommandValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(4000).WithErrorCode(ErrorCodes.TooLong);
    }
}