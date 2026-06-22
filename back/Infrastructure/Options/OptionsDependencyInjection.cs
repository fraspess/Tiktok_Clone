using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Options;

public static class OptionsDependencyInjection
{
    public static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<EmailOptions>()
            .BindConfiguration(EmailOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<FrontendOptions>()
            .BindConfiguration(FrontendOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<GoogleOptions>()
            .BindConfiguration(GoogleOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<RabbitMQOptions>()
            .BindConfiguration(RabbitMQOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        /*services.AddOptions<BackendUrlOptions>()
            .BindConfiguration("Backend")
            .ValidateDataAnnotations()
            .ValidateOnStart(); */

        services.AddOptions<AwsS3Options>()
            .BindConfiguration(AwsS3Options.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<RedisOptions>()
            .BindConfiguration(RedisOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}