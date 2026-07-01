using System.Text.Json.Serialization;

namespace Application.Dtos.User;

public class GoogleTokenExchangeResponse
{
    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }

    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}