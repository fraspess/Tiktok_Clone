using Application.Interfaces;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<IAppDbContext, AppDbContext>();

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

            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}