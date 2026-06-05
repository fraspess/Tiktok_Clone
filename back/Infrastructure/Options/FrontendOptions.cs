using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class FrontendOptions
{
    [Required]
    public string Url { get; set; }
}