using FluentValidation;

namespace Application.Features.Video.Upload
{
    public class UploadVideoCommandValidator : AbstractValidator<UploadVideoCommand>
    {
        public UploadVideoCommandValidator()
        {
            RuleFor(x => x.Dto.Description)
                .NotNull().WithMessage("Опис не може бути порожнім")
                .NotEmpty().WithMessage("Опис відео не може бути порожнім")
                .MaximumLength(500);

        }
    }
}