using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class AwsS3Options
{
    [Required]
    public string Url { get; set; }
}