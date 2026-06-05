using Application.Dtos.Token;
using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Token
{
    internal class JWTTokenService(IOptions<JwtOptions> settings, UserManager<UserEntity> userManager) : IJWTTokenService
    {
        private readonly JwtOptions _options = settings.Value;


        public async Task<TokenResponseDTO> GenerateTokensAsync(UserEntity user)
        {
            return new TokenResponseDTO
            {
                AccessToken = await CreateAccessTokenAsync(user),
                RefreshToken = await CreateRefreshTokenAsync(user)
            };
        }

        public async Task<TokenResponseDTO> RefreshTokensAsync(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_options.Key
                    )),
            };

            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out _);
            }
            catch (Exception)
            {
                throw new UnauthorizedException("Не валідний refresh токен");
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? throw new UnauthorizedException("Не валідний refresh токен");

            var tokenVersion = principal.FindFirst("Version")?.Value
                               ?? throw new UnauthorizedException("Не валідний refresh токен");

            var user = userManager.Users.FirstOrDefault(u => u.Id.ToString() == userId)
                       ?? throw new UnauthorizedException("Користувача не знайдено");

            if (user.RefreshTokenVersion != int.Parse(tokenVersion))
            {
                throw new UnauthorizedException("Не валідний refresh токен");
            }

            if (user.IsBanned is true)
            {
                throw new NotAllowedException("Аккаунт заблокований");
            }

            return await GenerateTokensAsync(user);
        }

        private async Task<string> CreateAccessTokenAsync(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("email", user.Email ?? ""),
                new Claim("username", user.UserName ?? "")
            };

            foreach (var role in await userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim("role", role));
            }

            var signingCredentials = GetSigningCredentials();

            var accessToken = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpiryMinutes),
                signingCredentials: signingCredentials
            );

            string accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            return accessTokenString;
        }


        private async Task<string> CreateRefreshTokenAsync(UserEntity user)
        {
            var claims = new List<Claim>()
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("Version", user.RefreshTokenVersion.ToString()),
            };

            var signingCredentials = GetSigningCredentials();

            var refreshToken = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToInt32(_options.RefreshTokenExpiryDays)),
                signingCredentials: signingCredentials
            );

            string refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);

            return refreshTokenString;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(_options.Key);
            var signingInKey = new SymmetricSecurityKey(keyBytes);
            return new SigningCredentials(signingInKey, SecurityAlgorithms.HmacSha256);
        }
    }
}