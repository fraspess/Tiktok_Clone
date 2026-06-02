using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class FrontendOptions
{
    [Required]
    public string Url { get; set; }
}