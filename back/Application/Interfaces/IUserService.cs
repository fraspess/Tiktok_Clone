using Application.Dtos.Token;
using Application.Dtos.User;

namespace Application.Interfaces;

public interface IUserService
{
    Task<TokenResponseDTO> Login(LoginUserDto dto);

    Task Register(RegisterUserDto dto);

    Task<TokenResponseDTO> ConfirmEmail(string email, string token);

    Task UpdateTokenVersion(Guid userId);

    Task ForgotPasswordAsync(string email);

    Task ResetPasswordAsync(ResetPasswordDto dto);

    Task ResendConfirmationEmailAsync(string email);

    Task<TokenResponseDTO> GoogleAuth(string code);
}