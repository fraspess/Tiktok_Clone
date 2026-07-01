using Domain.Exceptions;
using FluentValidation;

namespace Application.Features.Video.GetBySomeQuery;

public class GetVideosBySomeStringValidator : AbstractValidator<GetVideosBySomeStringQuery>
{
    public GetVideosBySomeStringValidator()
    {
        RuleFor(v => v.SomeString)
            .NotEmpty().WithErrorCode(ErrorCodes.Required);
    }
}