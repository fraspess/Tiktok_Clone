using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class AwsS3Options
{
    [Required]
    public string ImagesBucketName { get; set; }
    [Required]
    public string UploadsBucketName { get; set; }
    [Required]
    public string VideoCdnDomain { get; set; } = default!;
    [Required]
    public string AvatarsCdnDomain { get; set; } = default!;
    [Required]
    public string CdnScheme { get; set; } = "https";
}