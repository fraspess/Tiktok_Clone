using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class GoogleOptions
{
    public const string SectionName = "Google";
    [Required] public required string ClientId { get; set; }

    [Required] public required string ClientSecret { get; set; }
}