using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class GoogleOptions
{
    [Required]
    public string ClientId { get; set; }
    
    [Required]
    public string ClientSecret { get; set; }
}