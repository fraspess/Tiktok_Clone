using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class RabbitMQOptions
{
    [Required]
    public string HostName { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}