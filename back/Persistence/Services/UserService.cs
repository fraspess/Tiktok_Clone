using System.Net.Http.Json;
using Application.Constants;
using Application.Dtos.Token;
using Application.Dtos.User;
using Application.Extensions;
using Application.Interfaces;
using Application.Mapper;
using Application.Options;
using Domain.Entities.Identity;
using Domain.Exceptions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Persistence.Services;

internal class UserService(
    UserManager<UserEntity> userManager,
    IJWTTokenService tokenService,
    IImageService imageService,
    IConfiguration configuration,
    IEmailService emailService,
    UserMapper mapper,
    HttpClient httpClient,
    ICurrentUser currentUser,
    IOptions<GoogleOptions> options)
    : IUserService
{
    private readonly GoogleOptions _googleOptions = options.Value;
    private string GetHtmlTemplate(string templateName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", templateName);
        return File.ReadAllText(path);
    }

    public async Task<TokenResponseDTO> Login(LoginUserDto dto)
    {
        var user =
            await userManager.Users.FirstOrDefaultAsync(u => u.UserName == dto.Login || u.Email == dto.Login)
            ?? throw new BadRequestException("Невірний логін або пароль");

        var checkPassword = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!checkPassword) throw new BadRequestException("Невірний логін або пароль");

        if (!user.EmailConfirmed) throw new NotAllowedException("Підтвердіть свою електронну пошту, щоб увійти");

        if (user.IsBanned is true) throw new NotAllowedException("Аккаунт був заблокований");

        return await tokenService.GenerateTokensAsync(user);
    }

    public async Task Register(RegisterUserDto dto)
    {
        var isEmailTaken = await userManager.Users.AnyAsync(u => u.Email == dto.Email);
        if (isEmailTaken) throw new BadRequestException("Почта вже занята");
        var isUsernameTaken = await userManager.Users.AnyAsync(u => u.UserName == dto.Username);
        if (isUsernameTaken) throw new BadRequestException("Ім'я користувача вже заняте");

        var user = mapper.ToEntity(dto);

        var result = await userManager.CreateAsync(user, dto.Password);
        if (result.Succeeded)
        {
            if (dto.Avatar is not null) await imageService.SaveImageAsync(dto.Avatar, user.Id);

            await userManager.AddToRoleAsync(user, RoleNames.USER_ROLE);

            user.LastConfirmationEmailSentAt = DateTime.UtcNow;
            await userManager.UpdateAsync(user);
            await GenerateTokenAndSendConfirmationEmailAsync(user);
        }
        else
        {
            throw new Exception("Помилка при створенні користувача : " +
                                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task UpdateTokenVersion(Guid userId)
    {
        var user = userManager.Users.FirstOrDefault(u => u.Id == userId)
                   ?? throw new UnauthorizedException("Користувач не знайдений");

        var currentVersion = user.RefreshTokenVersion;
        user.RefreshTokenVersion = currentVersion + 1;

        await userManager.UpdateAsync(user);
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email)
                   ?? throw new UnauthorizedException("Користувач не знайдений");

        if (!await userManager.HasPasswordAsync(user))
            throw new BadRequestException("Аккаунтам створених зовнішними сервісами не можливо сбросити пароль");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"{configuration["Frontend:Url"]}/reset-password?token={token}&email={email}";
        var body = GetHtmlTemplate("ResetPassword.html");
        body = body.Replace("{resetLink}", resetLink);

        await emailService.SendEmailAsync(email, "Скидання пароля", body);
    }

    public async Task<TokenResponseDTO> ConfirmEmail(string email, string token)
    {
        var user = userManager.Users.FirstOrDefault(u => u.Email == email)
                   ?? throw new UnauthorizedException("Користувач не знайдений");

        if (user.EmailConfirmed == true) throw new BadRequestException("Пошта вже підтверджена");

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            return await tokenService.GenerateTokensAsync(user);
        else
            throw new BadRequestException("Невірний токен підтвердження");
    }

    // Скидає пароль і міняє версію токен на + 1 щоб інші токени стали недійсними
    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = userManager.Users.FirstOrDefault(u => u.Email == dto.Email)
                   ?? throw new UnauthorizedException("Користувач не знайдений");

        var result = await userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        if (result.Succeeded)
            await UpdateTokenVersion(user.Id);
        else
            throw new BadRequestException("Невірний токен для скидання пароля");
    }

    public async Task ResendConfirmationEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email)
                   ?? throw new NotFoundException("Почту не знайдено");

        if (user.EmailConfirmed) throw new BadRequestException("Почту уже підтвердженно");

        if (user.LastConfirmationEmailSentAt.HasValue)
        {
            var timePassed = DateTime.UtcNow - user.LastConfirmationEmailSentAt.Value;
            if (timePassed.TotalMinutes < 5)
            {
                var remaining = 5 - (int)timePassed.TotalMinutes;
                throw new BadRequestException($"Повторіть спробу через {remaining} хвилин ");
            }
        }

        user.LastConfirmationEmailSentAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        await GenerateTokenAndSendConfirmationEmailAsync(user);
    }


    private async Task GenerateTokenAndSendConfirmationEmailAsync(UserEntity user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var body = GetHtmlTemplate("ConfirmEmail.html");
        body = body.Replace("{confirmCode}", token);
        await emailService.SendEmailAsync(user.Email!, "Підтвердження реєстрації", body);
    }

    public async Task<TokenResponseDTO> GoogleAuth(string code)
    {
        var tokenResponse = await httpClient.PostAsync(
            "https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", code },
                {"client_id", _googleOptions.ClientId},
                {"client_secret", _googleOptions.ClientSecret},
                {"redirect_uri", "postmessage"},
                {"grant_type", "authorization_code"}
            }));

        if (!tokenResponse.IsSuccessStatusCode)
            throw new UnauthorizedException("Помилка при вході через гугл. Спробуйте ще раз.");

        var tokenJson = await tokenResponse.Content.ReadFromJsonAsync<GoogleTokenExchangeResponse>();

        if (tokenJson?.IdToken is null)
            throw new UnauthorizedException("Помилка при вході через гугл. Спробуйте ще раз.");
        
        var idToken = tokenJson.IdToken;
                
        GoogleJsonWebSignature.Payload payload;
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { configuration["Google:ClientId"] }
            };
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (InvalidJwtException)
        {
            throw new UnauthorizedException("Помилка при вході через гугл. Спробуйте ще раз.");
        }

        if (!payload.EmailVerified)
            throw new UnauthorizedException("Помилка при валідації почти. Спробуйте ще раз.");

        var existingUser = await userManager.FindByLoginAsync("Google", payload.Subject);
        if (existingUser is not null)
            return await tokenService.GenerateTokensAsync(existingUser);

        var user = await userManager.FindByEmailAsync(payload.Email);
        if (user is not null)
        {
            await userManager.AddLoginAsync(user, new UserLoginInfo(
                "Google",
                payload.Subject,
                "Google"
            ));

            return await tokenService.GenerateTokensAsync(user);
        }

        var baseUsername = payload.Email.Split('@')[0];
        var username = baseUsername;
        var counter = 1;

        while (await userManager.FindByNameAsync(username) is not null) username = $"{baseUsername}{counter++}";

        user = new UserEntity
        {
            Email = payload.Email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
            EmailConfirmed = true,
            UserName = username
        };

        await userManager.CreateAsync(user);
        if (!string.IsNullOrEmpty(payload.Picture)) await imageService.SaveImageAsync(payload.Picture, user.Id);

        await userManager.AddToRoleAsync(user, RoleNames.USER_ROLE);
        await userManager.AddLoginAsync(user, new UserLoginInfo(
            "Google",
            payload.Subject,
            "Google"
        ));

        return await tokenService.GenerateTokensAsync(user);
    }
}