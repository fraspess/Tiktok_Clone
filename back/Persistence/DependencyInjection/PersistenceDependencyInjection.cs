using Application.Interfaces;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories.Comment;
using Persistence.Repositories.Conversation;
using Persistence.Repositories.Favorite;
using Persistence.Repositories.Follow;
using Persistence.Repositories.HashTag;
using Persistence.Repositories.Like;
using Persistence.Repositories.Message;
using Persistence.Repositories.Video;
using Persistence.Services;

namespace Persistence.DependencyInjection
{
    public static class PersistenceDependencyInjection
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<AuditingSaveChanges>();
            services.AddDbContext<AppDbContext>((serviceProvider,options) =>
            {
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")).AddInterceptors(serviceProvider.GetRequiredService<AuditingSaveChanges>());
            });

            services.AddIdentityCore<UserEntity>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;

                    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                })
                .AddRoles<RoleEntity>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddDataProtection();

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromMinutes(30);
            });

            services.AddScoped<IVideoRepository, VideoRepository>();
            services.AddScoped<IHashTagRepository, HashTagRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IFollowRepository, FollowRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}