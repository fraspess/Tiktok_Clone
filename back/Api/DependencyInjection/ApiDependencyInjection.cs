using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json.Serialization;
using Api.Filters;
using Api.RateLimiting;
using Microsoft.AspNetCore.OpenApi;

namespace Api.DependencyInjection;

public static class ApiDependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration config,
        IWebHostEnvironment env)
    {
        services.AddControllers(opt =>
            {
                opt.Filters.AddService<RateLimitFilter>();
                opt.Filters.Add<NullActionFilter>();
            })
            .ConfigureApiBehaviorOptions(opt => { opt.SuppressModelStateInvalidFilter = true; })
            .AddJsonOptions(opts => { opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });


        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["Jwt:Key"]!
                        ))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var tokenType = context.Principal?.FindFirst("type")?.Value;

                        if (tokenType != "access")
                        {
                            context.Fail("Invalid token type.");
                        }

                        return Task.CompletedTask;
                    },
                    
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AccessToken", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("type", "access");
            });
        });

        if (env.IsDevelopment())
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy => policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });
        else
            services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(config["Frontend:Url"]!)
                        .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS", "PATCH")
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

        services.AddHealthChecks();
 
        services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });

            opt.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });

            opt.UseInlineDefinitionsForEnums();
        });

        services.AddSingleton<SlidingWindowRateLimiter>();
        services.AddScoped<RateLimitFilter>();


        return services;
    }
}