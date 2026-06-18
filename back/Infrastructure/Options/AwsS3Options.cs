using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class AwsS3Options
{
    public const string SectionName = "AWS:S3";
    [Required]
    public string BucketName { get; set; }

    [Required] public string CdnBaseUrl { get; set; } = "http://localhost:4566";
    public string? ServiceUrl { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? Region { get; set; }
}