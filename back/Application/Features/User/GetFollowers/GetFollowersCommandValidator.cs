using Application.Extensions;
using Domain.Exceptions;
using FluentValidation;

namespace Application.Features.User.GetFollowers;

public class GetFollowersCommandValidator : AbstractValidator<GetUserFollowersCommand>
{
    public GetFollowersCommandValidator()
    {
        RuleFor(c => c.Username).IsValidUsername().WithErrorCode(ErrorCodes.InvalidUsername);
    }
}