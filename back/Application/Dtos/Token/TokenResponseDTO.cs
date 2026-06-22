namespace Application.Dtos.Token;

public class TokenResponseDTO
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}