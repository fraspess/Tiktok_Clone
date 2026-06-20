using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class GoogleOptions
{
    public const string SectionName = "Google";
    [Required] public string ClientId { get; set; }

    [Required] public string ClientSecret { get; set; }
}