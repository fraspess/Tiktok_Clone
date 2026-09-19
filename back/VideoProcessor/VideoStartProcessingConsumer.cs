using System.Text;
using Amazon.S3;
using Amazon.S3.Transfer;
using Contracts.Events;
using FFMpegCore;
using MassTransit;
using Microsoft.Extensions.Options;

namespace VideoProcessor;

internal class VideoStartProcessingConsumer(
    ILogger<VideoStartProcessingConsumer> logger,
    IOptions<FfmpegOptions> ffmpegOptions,
    IVideoFileStorage storage,
    IPublishEndpoint publishEndpoint
    ) : IConsumer<VideoStartProcessingEvent>
{
    private readonly FfmpegOptions _opts = ffmpegOptions.Value;

    public async Task Consume(ConsumeContext<VideoStartProcessingEvent> context)
    {
        var videoId = context.Message.VideoId;
        var tempPath = Path.Combine(Path.GetTempPath(), "unprocessed", videoId.ToString());
        var outputPath = Path.Combine(Path.GetTempPath(), "processed", videoId.ToString());
        var inputPath = Path.Combine(tempPath, "original");
        var outputDir = outputPath;
        var normalizedPath = Path.Combine(outputDir, "normalized.mp4");
        try
        {
            Directory.CreateDirectory(tempPath);
            await storage.DownloadOriginalAsync(videoId, inputPath);

            logger.LogInformation("Started processing video {inputPath}", inputPath);

            var videoInfo = await ValidateVideoAsync(inputPath);
            var duration = videoInfo.Duration;
            var hasAudio = videoInfo.AudioStreams.Count > 0;

            if (!hasAudio)
                logger.LogInformation("Video {VideoId} has no audio stream, processing without audio", videoId);

            Directory.CreateDirectory(outputDir);

            await NormalizeVideoAsync(inputPath, normalizedPath, duration, videoId, hasAudio);
            await GenerateHlsAsync(normalizedPath, outputDir, duration, videoId, hasAudio);
            await GenerateThumbnailAsync(normalizedPath, outputDir, duration);

            if (File.Exists(normalizedPath))
                File.Delete(normalizedPath);

            await storage.UploadProcessedAsync(videoId, outputDir);

            await publishEndpoint.Publish(new VideoProcessedEvent { VideoId = videoId });

            logger.LogInformation("Video successfully processed {outputPath}", outputPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to convert file: {Error}", ex.Message);
            await publishEndpoint.Publish(new VideoProcessingFailedEvent(videoId, ex.Message));
            throw;
        }
        finally
        {
            try
            {
                if (Directory.Exists(tempPath))
                    Directory.Delete(tempPath, true);

                if (Directory.Exists(outputPath))
                    Directory.Delete(outputPath, true);
            }
            catch (Exception cleanupEx)
            {
                logger.LogWarning(cleanupEx, "Failed to clean up temp files for video {VideoId}", videoId);
            }
        }
    }

    private static async Task<IMediaAnalysis> ValidateVideoAsync(string filePath)
    {
        IMediaAnalysis mediaInfo;
        try
        {
            mediaInfo = await FFProbe.AnalyseAsync(filePath);
        }
        catch
        {
            throw new Exception("Файл не є відео");
        }

        if (mediaInfo.VideoStreams.Count == 0)
            throw new Exception("Відео не має відеопотоків");

        if (mediaInfo.Duration <= TimeSpan.Zero)
            throw new Exception("Відео не може бути 0 секунд довжиною");

        if (mediaInfo.Duration > TimeSpan.FromHours(3))
            throw new Exception("Відео не може бути довше ніж 3 години");
        
        return mediaInfo;
    }

    private async Task NormalizeVideoAsync(string input, string output, TimeSpan duration, Guid videoId, bool hasAudio)
    {
        var filter =
            "split[orig][copy];" +
            "[copy]scale=1080:1920:force_original_aspect_ratio=increase,crop=1080:1920,boxblur=20[bg];" +
            "[orig]scale=1080:1920:force_original_aspect_ratio=decrease[fg];" +
            "[bg][fg]overlay=(W-w)/2:(H-h)/2[v]";

        var mapArgs = "-map [v]";

        if (hasAudio)
        {
            filter += ";[0:a]loudnorm=I=-14:TP=-1:LRA=11[a]";
            mapArgs += " -map [a]";
        }

        await FFMpegArguments
            .FromFileInput(input)
            .OutputToFile(output, true, options =>
            {
                options
                    .WithVideoCodec(_opts.Encoding.VideoCodec)
                    .WithCustomArgument($"-preset {_opts.Encoding.Preset}")
                    .WithConstantRateFactor(_opts.Encoding.Crf)
                    .WithCustomArgument($"-filter_complex \"{filter}\" {mapArgs}")
                    .WithFastStart();

                if (hasAudio)
                    options.WithAudioCodec(_opts.Encoding.AudioCodec);
                else
                    options.WithCustomArgument("-an");
            })
            .NotifyOnProgress((TimeSpan progress) =>
            {
                var percent = CalcPercent(progress, duration);
                PublishProgressSafe(videoId, percent / 2);
            })
            .ProcessAsynchronously();
    }

    private async Task GenerateHlsAsync(string input, string output, TimeSpan duration, Guid videoId, bool hasAudio)
    {
        var qualities = _opts.Qualities;

        for (var i = 0; i < qualities.Count; i++)
        {
            var q = qualities[i];
            var dir = Path.Combine(output, q.Quality.ToString());
            Directory.CreateDirectory(dir);

            var i1 = i;
            await FFMpegArguments
                .FromFileInput(input)
                .OutputToFile(Path.Combine(dir, "playlist.m3u8"), true, options =>
                {
                    options
                        .WithVideoCodec(_opts.Encoding.VideoCodec)
                        .WithCustomArgument($"-preset {_opts.Encoding.Preset}")
                        .WithCustomArgument($"-vf scale={q.Scale}")
                        .WithCustomArgument($"-b:v {q.VideoBitrate} -maxrate {q.MaxRate} -bufsize {q.BuffSize}")
                        .WithCustomArgument("-hls_time 4")
                        .WithCustomArgument("-hls_playlist_type vod")
                        .WithCustomArgument("-hls_flags independent_segments")
                        .WithCustomArgument($"-hls_segment_filename \"{Path.Combine(dir, "seg_%03d.ts")}\"")
                        .ForceFormat("hls");

                    if (hasAudio)
                        options.WithAudioCodec(_opts.Encoding.AudioCodec);
                    else
                        options.WithCustomArgument("-an");
                })
                .NotifyOnProgress((TimeSpan progress) =>
                {
                    var percent = CalcPercent(progress, duration);
                    var offset = 50 + i1 * (50 / qualities.Count);
                    var total = Math.Clamp(offset + percent / 2 / qualities.Count, 0, 100);
                    PublishProgressSafe(videoId, total);
                })
                .ProcessAsynchronously();
        }

        await WriteMasterPlaylistAsync(output);
    }

    private async Task WriteMasterPlaylistAsync(string output)
    {
        var sb = new StringBuilder("#EXTM3U\n#EXT-X-VERSION:3\n\n");
        foreach (var q in _opts.Qualities)
        {
            sb.AppendLine(
                $"#EXT-X-STREAM-INF:BANDWIDTH={q.Bandwidth},RESOLUTION={q.Scale.Replace(':', 'x')},NAME=\"{q.Quality}\"");
            sb.AppendLine($"{q.Quality}/playlist.m3u8");
        }

        await File.WriteAllTextAsync(Path.Combine(output, "master.m3u8"), sb.ToString());
    }

    private static async Task GenerateThumbnailAsync(string input, string output, TimeSpan duration)
    {
        var thumbPath = Path.Combine(output, "thumbnail.jpg");
        await FFMpeg.SnapshotAsync(input, thumbPath, captureTime: TimeSpan.FromSeconds(Math.Min(1, duration.TotalSeconds / 2)));
    }

    private static int CalcPercent(TimeSpan progress, TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            return 0;

        return Math.Clamp((int)Math.Floor(progress.TotalSeconds / duration.TotalSeconds * 100), 0, 100);
    }
    
    private void PublishProgressSafe(Guid videoId, int percent)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await publishEndpoint.Publish(new VideoProcessingProgressEvent(videoId, percent));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to publish progress for video {VideoId}", videoId);
            }
        });
    }
}