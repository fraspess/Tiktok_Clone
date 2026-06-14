using System.ComponentModel.DataAnnotations;

namespace VideoProcessor;

public class AwsS3Options
{
    [Required]
    public string BucketName { get; set; }
    
    public string? ServiceUrl { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? Region { get; set; }
}