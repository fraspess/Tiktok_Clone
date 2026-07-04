using System.ComponentModel.DataAnnotations;

namespace VideoProcessor;

public class AwsS3Options
{
    public string BucketName { get; set; } =  string.Empty;
    public string? ServiceUrl { get; set; } = string.Empty;
    public string? AccessKey { get; set; } =  string.Empty;
    public string? SecretKey { get; set; } = string.Empty;
    public string? Region { get; set; } = string.Empty;
}