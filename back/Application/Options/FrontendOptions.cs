using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class FrontendOptions
{
    public const string SectionName = "Frontend";
    [Required] public string Url { get; set; }
}