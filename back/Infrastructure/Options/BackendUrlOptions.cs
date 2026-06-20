using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class BackendUrlOptions
{
    [Required] public string Url { get; set; }
}