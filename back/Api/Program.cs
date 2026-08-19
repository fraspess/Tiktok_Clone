using Api.DependencyInjection;
using Api.Middleware;
using Application.DependencyInjection;
using Application.Interfaces;
using Application.Options;
using Infrastructure.DependencyInjection;
using Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Persistence;
using Persistence.DependencyInjection;
using Persistence.Seeder;
using Serilog;
using Serilog.Events;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    DotNetEnv.Env.Load();
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
    );

    builder.Services.AddApplication(builder.Configuration);
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddInfrastructure(builder, builder.Configuration);
    builder.Services.AddApi(builder.Configuration, builder.Environment);

    var app = builder.Build();
    app.MapHealthChecks("/health");
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tiktok-Clone"); });
    }

    app.UseMiddleware<GlobalExceptionHandler>();

    app.UseSerilogRequestLogging();

    app.UseCors();
    

    var localStorageOptions = app.Configuration.GetSection("LocalStorage").Get<LocalStorageOptions>();
    if (app.Environment.IsDevelopment() && localStorageOptions is not null)
    {
        var absoluteRoot = Path.GetFullPath(localStorageOptions.RootPath);
        Directory.CreateDirectory(absoluteRoot);
        var uploadsRoot =  Path.Combine(absoluteRoot, "uploads");
        Directory.CreateDirectory(uploadsRoot);
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadsRoot),
            RequestPath = "/uploads",
            ServeUnknownFileTypes = true,
            ContentTypeProvider = new FileExtensionContentTypeProvider
            {
                Mappings =
                {
                    [".m3u8"] = "application/vnd.apple.mpegurl",
                    [".ts"] = "video/mp2t"
                }
            }
        });
        
        var avatarsRoot = Path.Combine(absoluteRoot, "avatars");
        Directory.CreateDirectory(avatarsRoot);
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(avatarsRoot),
            RequestPath = "/avatars"
        });
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHub<ChatHub>("/hubs/chat");
    app.MapHub<VideoProcessingHub>("/hubs/video-process-status");

    if (app.Environment.IsDevelopment())
    {
        await app.SeedDataAsync();
    }
    else
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}