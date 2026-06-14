using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Options;

public static class OptionsDependencyInjection
{
    public static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<EmailOptions>()
            .BindConfiguration("SMTP")
            .ValidateDataAnnotations()
            .ValidateOnStart();
            
        services.AddOptions<JwtOptions>()
            .BindConfiguration("Jwt")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<FrontendOptions>()
            .BindConfiguration("Frontend")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<GoogleOptions>()
            .BindConfiguration("Google")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<RabbitMQOptions>()
            .BindConfiguration("RabbitMQ")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<BackendUrlOptions>()
            .BindConfiguration("Backend")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<AwsS3Options>()
            .BindConfiguration("AWS:S3")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        
        return services;
    }
}