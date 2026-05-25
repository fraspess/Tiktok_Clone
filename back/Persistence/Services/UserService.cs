using Application.Constants;
using Application.Dtos.Token;
using Application.Dtos.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.Identity;
using Domain.Exceptions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Services
{
    internal class UserService : IUserService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _userMapper;
        private readonly IJWTTokenService _jwtTokenService;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _uow;

        public UserService(UserManager<UserEntity> userManager,
            IMapper userMapper, IJWTTokenService tokenService,
            IImageService imageService,
            IConfiguration configuration,
            IEmailService emailService,
            IUnitOfWork uow)
        {
            _userManager = userManager;
            _userMapper = userMapper;
            _jwtTokenService = tokenService;
            _imageService = imageService;
            _configuration = configuration;
            _emailService = emailService;
            _uow = uow;
        }


        private string GetHtmlTemplate(string templateName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", templateName);
            return File.ReadAllText(path);
        }

        public async Task<TokenResponseDTO> Login(LoginUserDTO dto)
        {
            var user =
                await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == dto.Login || u.Email == dto.Login)
                ?? throw new BadRequestException("Невірний логін або пароль");

            var checkPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!checkPassword)
            {
                throw new ValidationException("Невірний логін або пароль");
            }

            if (!user.EmailConfirmed)
            {
                throw new NotAllowedException("Підтвердіть свою електронну пошту, щоб увійти");
            }

            if (user.IsBanned is true)
            {
                throw new NotAllowedException("Аккаунт був заблокований");
            }

            return await _jwtTokenService.GenerateTokensAsync(user);
        }

        public async Task Register(RegisterUserDTO dto)
        {
            var isEmailTaken = await _userManager.FindByEmailAsync(dto.Email)
                               ?? throw new BadRequestException("Почта вже є занятою");

            var isUsernameTaken = await _userManager.FindByNameAsync(dto.Username)
                                  ?? throw new BadRequestException("Це ім'я користувача заняте");

            var user = _userMapper.Map<UserEntity>(dto);

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                if (dto.Avatar is not null)
                {
                    var imageName = await _imageService.SaveImageAsync(dto.Avatar);
                    user.Avatar = imageName;
                }

                await _userManager.AddToRoleAsync(user, RoleNames.USER_ROLE);

                user.LastConfirmationEmailSentAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                await GenerateTokenAndSendAsync(user);
            }
            else
            {
                throw new Exception("Помилка при створенні користувача : " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<UserMeDTO> GetCurrentUserAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId)
                       ?? throw new UnauthorizedException("Користувач не знайдений");
            var dto = _userMapper.Map<UserMeDTO>(user);
            dto.FollowingCount = await _uow.Follows.GetFollowingCountAsync(userId);
            dto.FollowersCount = await _uow.Follows.GetFollowersCountAsync(userId);
            return dto;
        }

        public async Task UpdateTokenVersion(Guid userId)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId)
                       ?? throw new UnauthorizedException("Користувач не знайдений");

            var currentVersion = user.RefreshTokenVersion;
            user.RefreshTokenVersion = currentVersion + 1;

            await _userManager.UpdateAsync(user);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                       ?? throw new UnauthorizedException("Користувач не знайдений");

            if (!await _userManager.HasPasswordAsync(user))
                throw new BadRequestException("Аккаунтам створених зовнішними сервісами не можливо сбросити пароль");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string resetLink = $"{_configuration["Frontend:Url"]}/reset-password?token={token}&email={email}";
            var body = GetHtmlTemplate("ResetPassword.html");
            body = body.Replace("{resetLink}", resetLink);

            await _emailService.SendEmailAsync(email, "Скидання пароля", body);
        }

        public async Task<TokenResponseDTO> ConfirmEmail(string email, string token)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Email == email)
                       ?? throw new UnauthorizedException("Користувач не знайдений");

            if (user.EmailConfirmed == true)
            {
                throw new ValidationException("Пошта вже підтверджена");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return await _jwtTokenService.GenerateTokensAsync(user);
            }
            else
            {
                throw new ValidationException("Невірний токен підтвердження");
            }
        }

        // Скидає пароль і міняє версію токен на + 1 щоб інші токени стали недійсними
        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Email == dto.Email)
                       ?? throw new UnauthorizedException("Користувач не знайдений");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (result.Succeeded)
            {
                await UpdateTokenVersion(user.Id);
            }
            else
            {
                throw new ValidationException("Невірний токен для скидання пароля");
            }
        }

        public async Task ResendConfirmationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                       ?? throw new NotFoundException("Почту не знайдено");

            if (user.EmailConfirmed) throw new ValidationException("Почту уже підтвердженно");

            if (user.LastConfirmationEmailSentAt.HasValue)
            {
                var timePassed = DateTime.UtcNow - user.LastConfirmationEmailSentAt.Value;
                if (timePassed.TotalMinutes < 5)
                {
                    var remaining = 5 - (int)timePassed.TotalMinutes;
                    throw new ValidationException($"Повторіть спробу через {remaining} хвилин ");
                }
            }

            user.LastConfirmationEmailSentAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            await GenerateTokenAndSendAsync(user);
        }


        private async Task GenerateTokenAndSendAsync(UserEntity user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var body = GetHtmlTemplate("ConfirmEmail.html");
            body = body.Replace("{confirmCode}", token);
            await _emailService.SendEmailAsync(user.Email!, "Підтвердження реєстрації", body);
        }

        public async Task<UserDTO> GetByUsernameAsync(string username, Guid? currentUserId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username)
                       ?? throw new NotFoundException("Користувача з таким ім'ям не знайдено");

            var dto = _userMapper.Map<UserDTO>(user);
            dto.IsOwnProfile = currentUserId == dto.Id;
            dto.IsFollowing = currentUserId.HasValue &&
                              await _uow.Follows.IsFollowingAsync(currentUserId.Value, dto.Id);
            dto.FollowersCount = await _uow.Follows.GetFollowersCountAsync(dto.Id);
            dto.FollowingCount = await _uow.Follows.GetFollowingCountAsync(dto.Id);
            return dto;
        }

        public async Task ToggleFollowAsync(Guid follower, Guid following)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == follower)
                       ?? throw new UnauthorizedException("Користувача не знайдено");
            var isAlreadyFollowing = await _uow.Follows.GetFollowAsync(follower, following);

            if (isAlreadyFollowing is null)
            {
                user.Following.Add(new UserFollowEntity { FollowerId = follower, FollowingId = following });
            }
            else
            {
                user.Following.Remove(isAlreadyFollowing);
            }

            await _userManager.UpdateAsync(user);
        }

        public async Task<TokenResponseDTO> GoogleAuth(string idToken)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (InvalidJwtException)
            {
                throw new UnauthorizedException("Помилка при вході через гугл. Спробуйте ще раз.");
            }

            if (!payload.EmailVerified)
                throw new UnauthorizedException("Помилка при валідації почти. Спробуйте ще раз.");

            var existingUser = await _userManager.FindByLoginAsync("Google", payload.Subject);
            if (existingUser is not null)
                return await _jwtTokenService.GenerateTokensAsync(existingUser);

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user is not null)
            {
                await _userManager.AddLoginAsync(user, new UserLoginInfo(
                    "Google",
                    payload.Subject,
                    "Google"
                ));

                return await _jwtTokenService.GenerateTokensAsync(user);
            }

            var baseUsername = payload.Email.Split('@')[0];
            var username = baseUsername;
            var counter = 1;

            while (await _userManager.FindByNameAsync(username) is not null)
            {
                username = $"{baseUsername}{counter++}";
            }

            user = new UserEntity
            {
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                EmailConfirmed = true,
                UserName = username
            };

            await _userManager.CreateAsync(user);
            if (!string.IsNullOrEmpty(payload.Picture))
            {
                user.Avatar = await _imageService.SaveImageAsync(payload.Picture);
                await _userManager.UpdateAsync(user);
            }

            await _userManager.AddToRoleAsync(user, RoleNames.USER_ROLE);
            await _userManager.AddLoginAsync(user, new UserLoginInfo(
                "Google",
                payload.Subject,
                "Google"
            ));

            return await _jwtTokenService.GenerateTokensAsync(user);
        }

        public async Task<bool> IsExistsById(Guid id)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id) is not null;
        }
    }
}