using Api.RateLimiting;
using Application;
using Application.Dtos.User;
using Application.Extensions;
using Application.Features.User.ChangeUsername;
using Application.Features.User.ConfirmEmail;
using Application.Features.User.FollowUser;
using Application.Features.User.ForgotPassword;
using Application.Features.User.GetByUsername;
using Application.Features.User.GetCurrentUser;
using Application.Features.User.GetFollowers;
using Application.Features.User.GetFollowing;
using Application.Features.User.GoogleAuth;
using Application.Features.User.Login;
using Application.Features.User.LogOutOnAllDevices;
using Application.Features.User.RefreshTokens;
using Application.Features.User.Register;
using Application.Features.User.ResendConfirmationEmail;
using Application.Features.User.ResetPassword;
using Application.Features.User.Settings.ChangeMessagePrivacy;
using Application.Features.User.Settings.GetMessagePrivacy;
using Application.Features.User.Update;
using Application.Pagination;
using Domain.Constants;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.User;

[ApiController]
[Route("api/users")]
public class UserController(IMediator _mediator) : ControllerBase
{
    [HttpPost("login")]
    [RateLimit(10, 60_000)]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var tokens = await _mediator.Send(command);

        AppendRefreshTokenCookie(tokens.RefreshToken);
        return Ok(ApiResponse<object>.Success(new { accessToken = tokens.AccessToken }, "Успішний вхід"));
    }

    [RateLimit(10, 60_000)]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        await _mediator.Send(command);

        return Ok(ApiResponse<object>.Success(null!,
            "Код для підтвердження реєстрації був надісланий на вказану почту."));
    }
    
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        var tokens = await _mediator.Send(command);
        AppendRefreshTokenCookie(tokens.RefreshToken);
        return Ok(ApiResponse<object>.Success(new { accessToken = tokens.AccessToken }, "Пошта підтверджена."));
    }
    
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthDto request)
    {
        var tokens = await _mediator.Send(new GoogleAuthCommand(request.Code));
        AppendRefreshTokenCookie(tokens.RefreshToken);
        return Ok(ApiResponse<object>.Success(new { accessToken = tokens.AccessToken }));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"]
                           ?? throw new UnauthorizedException("Refresh token не знайдений");

        var newTokens = await _mediator.Send(new RefreshTokensCommand(refreshToken));

        AppendRefreshTokenCookie(newTokens.RefreshToken);

        return Ok(ApiResponse<object>.Success(new { accessToken = newTokens.AccessToken }, "Токени оновлено"));
    }

    [HttpPost("logout")]
    [Authorize]
    public Task<IActionResult> Logout()
    {
        DeleteRefreshTokenCookie();

        return Task.FromResult<IActionResult>(Ok(ApiResponse<object>.Success(null!, "Успішний вихід")));
    }

    [HttpPost("logout/all")]
    [Authorize]
    public async Task<IActionResult> LogoutAll()
    {
        await _mediator.Send(new LogOutOnAllDevicesCommand());

        DeleteRefreshTokenCookie();
        return Ok(ApiResponse<object>.Success(null!, "Успішний вихід з усіх пристроїв"));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand forgotPasswordCommand)
    {
        await _mediator.Send(forgotPasswordCommand);
        return Ok(ApiResponse<object>.Success(null!, "Перевірте вашу почту"));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse<object>.Success(null!, "Пароль успішно змінено"));
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse<object>.Success(null!, "Перевірте вашу почту"));
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserProfile(string username)
    {
        username = username.TrimStart('@');
        var profile = await _mediator.Send(new GetUserByUsernameQuery(username));
        return Ok(ApiResponse<UserDto>.Success(profile));
    }

    [HttpPost("follow")]
    [Authorize]
    public async Task<IActionResult> Follow(Guid following)
    {
        await _mediator.Send(new FollowUserCommand(following));
        return Ok(ApiResponse<object>.Success(null!));
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> Update([FromForm] UpdateUserDto dto)
    {
        await _mediator.Send(new UpdateUserCommand(dto));
        return Ok(ApiResponse<object>.Success(null!));
    }

    [HttpPatch("change-username")]
    [Authorize]
    public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameUserDto dto)
    {
        await _mediator.Send(new ChangeUsernameCommand(dto.NewUsername));
        return Ok(ApiResponse<object>.Success(null!));
    }

    [HttpGet("{username}/followers")]
    public async Task<IActionResult> GetFollowers(string username, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var followers = await _mediator.Send(new GetUserFollowersCommand(username,
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<object>.Success(followers));
    }

    [HttpGet("{username}/following")]
    public async Task<IActionResult> GetFollowing(string username, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var following = await _mediator.Send(new GetUserFollowingCommand(username,
            new PaginationSettings { PageNumber = pageNumber, PageSize = pageSize }));
        return Ok(ApiResponse<object>.Success(following));
    }

    [HttpPost("settings/message-privacy")]
    public async Task<IActionResult> ChangeMessagePrivacy(MessagePrivacy newPrivacy)
    {
        await _mediator.Send(new ChangeMessagePrivacyCommand(newPrivacy));
        return Ok(ApiResponse<object?>.Success(null));
    }

    [HttpGet("settings/message-privacy")]
    public async Task<IActionResult> GetMessagePrivacy()
    {
        var privacy = await _mediator.Send(new GetMessagePrivacyCommand());
        return Ok(ApiResponse<object>.Success(privacy));
    }

    private void AppendRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Append("refreshToken", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-1)
        });
    }
}