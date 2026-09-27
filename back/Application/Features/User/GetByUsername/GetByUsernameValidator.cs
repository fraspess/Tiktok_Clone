using Domain.Constants;
using FluentValidation;

namespace Application.Features.User.GetByUsername;

public class GetByUsernameValidator : AbstractValidator<GetUserByUsernameQuery>
{
    public GetByUsernameValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithErrorCode(ErrorCodes.UsernameRequired);
    }
}