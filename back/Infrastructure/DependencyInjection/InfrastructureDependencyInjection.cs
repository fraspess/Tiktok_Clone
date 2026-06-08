using Amazon;
using Amazon.S3;
using Application.Interfaces;
using Contracts;
using Infrastructure.Options;
using Infrastructure.RabbitMQ;
using Infrastructure.RabbitMQ.Consumers;
using Infrastructure.Services.Storage;
using Infrastructure.Services.Email;
using Infrastructure.Services.Images;
using Infrastructure.Services.TempVideoStorage;
using Infrastructure.Services.Token;
using Infrastructure.SignalR;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.DependencyInjection
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            
            services.AddSignalR();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IJWTTokenService, JWTTokenService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IJWTTokenService, JWTTokenService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IChatNotifier, ChatNotifier>();
            services.AddScoped<IVideoProcessingNotifier, VideoProcessingNotifier>();
            services.AddScoped<ITempVideoStorage, TempVideoStorage>();
            services.AddScoped(typeof(IEventBus<>), typeof(EventBus<>));
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<HttpClient>();
            services.AddScoped<IStorageService, S3StorageService>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<VideoProcessedConsumer>();
                x.AddConsumer<VideoProcessingProgressConsumer>();
                x.AddConsumer<VideoProcessingFailedConsumer>();
                
                    x.UsingRabbitMq((ctx, cfg) =>
                    {
                        cfg.Host(config["RabbitMQ:HostName"], h =>
                        {
                            h.Username(config["RabbitMQ:UserName"]!);
                            h.Password(config["RabbitMQ:Password"]!);
                        });

                        cfg.ConfigureEndpoints(ctx);
                    });
            });
            
            services.AddHttpContextAccessor();

            var awsOptions = config.GetAWSOptions();
            
            services.AddDefaultAWSOptions(awsOptions);
            
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
                        config["AWS:S3:Region"] ?? "us-east-1");
                }

                var accessKey = config["AWS:S3:AccessKey"];
                var secretKey = config["AWS:S3:SecretKey"];

                return !string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey)
                    ? new AmazonS3Client(accessKey, secretKey, config1) 
                    : new AmazonS3Client(config1); 
            });
            
            services.AddConfigOptions(config);
            return services;
        }
    }
}