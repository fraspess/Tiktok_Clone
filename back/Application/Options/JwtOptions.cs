using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class JwtOptions
{
    [Required]
    public required string Key { get; set; }
    [Required]
    public required string Issuer { get; set; }
    [Required]
    public required string Audience { get; set; }
    [Required]
    public int AccessTokenExpiryMinutes { get; set; }
    [Required]
    public int RefreshTokenExpiryDays { get; set; }
}