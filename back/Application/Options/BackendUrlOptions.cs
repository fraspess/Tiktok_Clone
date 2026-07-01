using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class BackendUrlOptions
{
    [Required] public string Url { get; set; }
}