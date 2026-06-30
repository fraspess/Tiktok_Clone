using Application.Extensions;
using Domain.Exceptions;
using FluentValidation;

namespace Application.Features.User.GetFollowing;

public class GetUserFollowingCommandValidator : AbstractValidator<GetUserFollowingCommand>
{
    public GetUserFollowingCommandValidator()
    {
        RuleFor(x => x.Username).IsValidUsername().WithErrorCode(ErrorCodes.InvalidUsername);
    }
}