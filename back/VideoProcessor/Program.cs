using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using FFMpegCore;
using FFMpegCore.Extensions.Downloader;
using MassTransit;
using Microsoft.Extensions.Options;
using VideoProcessor;

DotNetEnv.Env.Load("../.env");
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddOptions<FFmpegOptions>()
    .BindConfiguration("FFmpeg")
    .ValidateDataAnnotations()
    .ValidateOnStart();
;

builder.Services.AddMassTransit(x =>
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
});

builder.Services.AddOptions<AwsS3Options>()
    .BindConfiguration("AWS:S3")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IAmazonS3>(sp =>
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

    return new AmazonS3Client(credentials, config1);
});

if (builder.Environment.IsDevelopment())
{
    var ffmpegPath = Path.Combine(Path.GetTempPath(), "ffmpeg.exe");
    GlobalFFOptions.Configure(new FFOptions { BinaryFolder = Path.GetTempPath() });
    if (!File.Exists(ffmpegPath)) await FFMpegDownloader.DownloadBinaries();
}

Console.WriteLine("ENVIRONMENT :: ", builder.Environment);

var host = builder.Build();

host.Run();