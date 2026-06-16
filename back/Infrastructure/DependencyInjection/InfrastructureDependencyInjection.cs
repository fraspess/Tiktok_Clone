using Amazon;
using Amazon.Runtime;
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

                return new AmazonS3Client(credentials,config1); 
            });
            
            services.AddConfigOptions(config);
            return services;
        }
    }
}