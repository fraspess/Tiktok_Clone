using FFMpegCore;
using FFMpegCore.Extensions.Downloader;
using MassTransit;
using VideoProcessor;

DotNetEnv.Env.Load("../.env");
var builder = Host.CreateApplicationBuilder(args);

/*builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<VideoStartProcessingConsumer>();
    
        x.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(builder.Configuration["RabbitMQ:HostName"], h =>
            {
                h.Username(builder.Configuration["RabbitMQ:UserName"]!);
                h.Password(builder.Configuration["RabbitMQ:Password"]!);
            });

            cfg.UseConcurrencyLimit(1);
            cfg.ConfigureEndpoints(ctx);
        });
    
});*/

if (builder.Environment.IsDevelopment())
{
    var ffmpegPath = Path.Combine(Path.GetTempPath(), "ffmpeg.exe");
    GlobalFFOptions.Configure(new FFOptions { BinaryFolder = Path.GetTempPath() });
    if (!File.Exists(ffmpegPath))
    {
        await FFMpegDownloader.DownloadBinaries();
    }
}

builder.Services.AddOptions<FFmpegOptions>()
    .BindConfiguration("FFmpeg")
    .ValidateDataAnnotations()
    .ValidateOnStart();
var host = builder.Build();

host.Run();