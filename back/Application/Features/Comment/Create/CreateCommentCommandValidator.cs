using FluentValidation;

namespace Application.Features.Comment.Create;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.Dto.Text)
            .NotEmpty().WithMessage("Коментарій не може бути пустим.")
            .MaximumLength(200).WithMessage("Коментарій не може бути більше ніж 200 символів.");

        RuleFor(x => x.Dto.VideoId)
            .NotEmpty().WithMessage("VideoId?????");
    }
}