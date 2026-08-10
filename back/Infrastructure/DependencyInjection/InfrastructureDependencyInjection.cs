using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Application.Interfaces;
using Application.Options;
using Contracts;
using Infrastructure.RabbitMQ;
using Infrastructure.RabbitMQ.Consumers;
using Infrastructure.Services;
using Infrastructure.Services.Storage;
using Infrastructure.Services.Email;
using Infrastructure.Services.Images;
using Infrastructure.Services.Token;
using Infrastructure.SignalR;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, WebApplicationBuilder builder, IConfiguration config)
    {
        services.AddSignalR();
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IChatNotifier, ChatNotifier>();
        services.AddScoped<IVideoProcessingNotifier, VideoProcessingNotifier>();
        services.AddScoped(typeof(IEventBus<>), typeof(EventBus<>));
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<HttpClient>();
        
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.Configure<LocalStorageOptions>(builder.Configuration.GetSection("LocalStorage"));
            builder.Services.AddScoped<IStorageService, LocalFileStorageService>();
            services.AddScoped<IImageService, LocalImageService>();
        }
        else
        {
            builder.Services.Configure<AwsS3Options>(builder.Configuration.GetSection("AWS:S3"));
            builder.Services.AddScoped<IStorageService, S3StorageService>();
            services.AddScoped<IImageService, ImageService>();
        }
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<VideoProcessedConsumer>();
            x.AddConsumer<VideoProcessingProgressConsumer>();
            x.AddConsumer<VideoProcessingFailedConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var options = ctx.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                cfg.Host(options.HostName, h =>
                {
                    h.Username(options.UserName);
                    h.Password(options.Password);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddHttpContextAccessor();

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var aws = sp.GetRequiredService<IOptions<AwsS3Options>>().Value;
            var config1 = new AmazonS3Config();

            if (!string.IsNullOrEmpty(aws.ServiceUrl))
            {
                config1.ServiceURL = aws.ServiceUrl;
                config1.ForcePathStyle = true;
            }
            else
            {
                config1.RegionEndpoint = RegionEndpoint.GetBySystemName(
                    aws.Region ?? "eu-central-1");
            }

            var accessKey = aws.AccessKey;
            var secretKey = aws.SecretKey;
            var credentials = new BasicAWSCredentials(accessKey, secretKey);

            return new AmazonS3Client(credentials, config1);
        });

        var redisOptions = config.GetSection(RedisOptions.SectionName).Get<RedisOptions>()!;
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisOptions.ConnectionString;
            options.InstanceName = redisOptions.InstanceName;
        });

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var configOptions = ConfigurationOptions.Parse(redisOptions.ConnectionString);
            configOptions.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(configOptions);
        });
        services.AddScoped<ICacheService, CacheService>();
        
        return services;
    }
}