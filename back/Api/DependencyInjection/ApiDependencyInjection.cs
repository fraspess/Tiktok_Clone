using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json.Serialization;
using Api.Filters;

namespace Api.DependencyInjection
{
    public static class ApiDependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration config,
            IWebHostEnvironment env)
        {
            services.AddControllers(opt => opt.Filters.Add<NullActionFilter>())
                .ConfigureApiBehaviorOptions(opt => { opt.SuppressModelStateInvalidFilter = true; })
                .AddJsonOptions(opts => {
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());});
            
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
                            )),
                    };

                    options.Events = new JwtBearerEvents
                    {
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

            services.AddAuthorization();

            if (env.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(policy => policy
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
                });
            }
            else
            {
                services.AddCors(opt =>
                {
                    opt.AddDefaultPolicy(policy =>
                    {
                        policy.WithOrigins(config["Frontend:Url"]!)
                            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                });
            }

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

            

            return services;
        }
    }
}