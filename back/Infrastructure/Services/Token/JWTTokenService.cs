using Application.Dtos.Token;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Features.Video.Upload.CompleteUpload;
using Application.Options;
using Domain.Constants;
using Domain.Entities.Identity;
using Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Token;

internal class JwtTokenService(IOptions<JwtOptions> settings, UserManager<UserEntity> userManager) : IJwtTokenService
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
            ClockSkew = TimeSpan.Zero
        };

        ClaimsPrincipal principal;
        try
        {
            principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out _);
        }
        catch (Exception)
        {
            throw new UnauthorizedException(ErrorCodes.InvalidToken, "The refresh token is invalid.");
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedException(ErrorCodes.InvalidToken, "The refresh token is invalid.");

        var tokenVersion = principal.FindFirst("version")?.Value
                           ?? throw new UnauthorizedException(ErrorCodes.InvalidToken, "The refresh token is invalid.");

        var user = userManager.Users.FirstOrDefault(u => u.Id.ToString() == userId)
                   ?? throw new UnauthorizedException(ErrorCodes.UserNotFound, "User not found.");

        if (user.RefreshTokenVersion != int.Parse(tokenVersion))
            throw new UnauthorizedException(ErrorCodes.InvalidToken, "The refresh token is invalid.");

        if (user.IsBanned) throw new NotAllowedException(ErrorCodes.UserBanned, "This account has been banned.");

        return await GenerateTokensAsync(user);
    }

    public string GenerateUploadToken(Guid videoId, Guid userId)
    {
        var claims = new[] { new Claim("videoId", videoId.ToString()), new Claim("userId", userId.ToString()) };
        
        var creds = GetSigningCredentials();

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public UploadTokenPayload ValidateUpdateToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            handler.ValidateToken(token, new TokenValidationParameters
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
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);
            
            var jwt = (JwtSecurityToken)validatedToken;
            return new UploadTokenPayload(Guid.Parse(jwt.Claims.First(c => c.Type == "videoId").Value), Guid.Parse(jwt.Claims.First(c => c.Type == "userId").Value));
        }
        catch(Exception ex)
        {
            return null;
        }
    }

    private async Task<string> CreateAccessTokenAsync(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("email", user.Email ?? ""),
            new("type", "access")
        };
        
        claims.AddRange(from role in await userManager.GetRolesAsync(user) select new Claim("role", role));

        var signingCredentials = GetSigningCredentials();

        var accessToken = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpiryMinutes),
            signingCredentials: signingCredentials
        );

        var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

        return accessTokenString;
    }


    private Task<string> CreateRefreshTokenAsync(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("version", user.RefreshTokenVersion.ToString()),
            new("type", "refresh")
        };

        var signingCredentials = GetSigningCredentials();

        var refreshToken = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: DateTime.UtcNow.AddDays(Convert.ToInt32(_options.RefreshTokenExpiryDays)),
            signingCredentials: signingCredentials
        );

        var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);

        return Task.FromResult(refreshTokenString);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var keyBytes = Encoding.UTF8.GetBytes(_options.Key);
        var signingInKey = new SymmetricSecurityKey(keyBytes);
        return new SigningCredentials(signingInKey, SecurityAlgorithms.HmacSha256);
    }
}
