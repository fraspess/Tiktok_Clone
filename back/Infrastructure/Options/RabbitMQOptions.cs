using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class RabbitMQOptions
{
    public const string SectionName = "RabbitMQ";
    [Required]
    public string HostName { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}