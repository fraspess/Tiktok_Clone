using Api.DependencyInjection;
using Api.Middleware;
using Application.DependencyInjection;
using Application.Interfaces;
using Infrastructure.DependencyInjection;
using Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
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
    DotNetEnv.Env.Load("../.env");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
    );

    builder.Services.AddApplication(builder.Configuration);
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
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

    var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
    Directory.CreateDirectory(uploadsPath);
    app.UseStaticFiles(new StaticFileOptions
    {
        RequestPath = "/uploads",
        FileProvider = new PhysicalFileProvider(uploadsPath),
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

    var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "images");
    Directory.CreateDirectory(imagesPath);
    app.UseStaticFiles(new StaticFileOptions
    {
        RequestPath = "/user-images",
        FileProvider = new PhysicalFileProvider(imagesPath)
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHub<ChatHub>("/hubs/chat");
    app.MapHub<VideoProcessingHub>("/hubs/video-process-status");

    // also auto-migrates
    await app.SeedDataAsync();

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