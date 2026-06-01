using Application.Behaviors;
using Application.Features.Video.Shared;
using Application.Mapper;
using Application.Options;
using Application.Services.HashTag;
using Application.Services.Message;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IHashTagService, HashTagService>();

            services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddMediatR(opt =>
            {
                opt.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
            });


            services.AddScoped<IDescriptionParser, DescriptionParser>();

            services.AddScoped<UserMapper>();
            services.AddScoped<VideoMapper>();
            services.AddScoped<ConversationMapper>();
            services.AddScoped<MessageMapper>();
            services.AddScoped<CommentMapper>();

            services.AddConfigOptions(config);
            return services;
        }
    }
}