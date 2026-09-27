using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class RedisOptions
{
    public const string SectionName = "Redis";
    [Required] public string ConnectionString { get; set; } = string.Empty;
    [Required] public string InstanceName { get; set; } = string.Empty;
}