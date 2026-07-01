using Domain.Exceptions;
using FluentValidation;

namespace Application.Features.Comment.Create;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.Dto.Text)
            .NotEmpty().WithErrorCode(ErrorCodes.Required)
            .MaximumLength(500).WithErrorCode(ErrorCodes.TooLong);
    }
}