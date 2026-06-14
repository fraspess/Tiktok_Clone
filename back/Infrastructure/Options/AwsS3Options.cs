using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class AwsS3Options
{
    [Required]
    public string BucketName { get; set; }
    [Required]
    public string CdnDomain { get; set; } = default!;
    public string CdnScheme { get; set; } = "https";
    public string? ServiceUrl { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? Region { get; set; }
}