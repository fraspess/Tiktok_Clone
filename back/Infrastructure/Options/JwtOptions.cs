using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    [Required, MinLength(32)]
    public string Key { get; set; }
    [Required]
    public string Issuer { get; set; }
    [Required]
    public string Audience { get; set; }
    [Required]
    public int AccessTokenExpiryMinutes { get; set; }
    [Required]
    public int RefreshTokenExpiryDays { get; set; }
}