using FluentValidation;

namespace Application.Features.Video.GetBySomeQuery;

public class GetVideosBySomeStringValidator : AbstractValidator<GetVideosBySomeStringQuery>
{
    public GetVideosBySomeStringValidator()
    {
        RuleFor(v => v.SomeString)
            .NotEmpty().WithMessage("Query не може бути порожній");
    }
}