/*using System.Text;
using Contracts.Events;
using FFMpegCore;
using MassTransit;
using Microsoft.Extensions.Options;

namespace VideoProcessor
{
    internal class VideoStartProcessingConsumer(
        ILogger<VideoStartProcessingConsumer> _logger,
        IOptions<FFmpegOptions> _fFmpegOptions,
        IPublishEndpoint publishEndpoint,
        IConfiguration config) : IConsumer<VideoStartProcessingEvent>
    {
        private readonly FFmpegOptions _opts = _fFmpegOptions.Value;

        public async Task Consume(ConsumeContext<VideoStartProcessingEvent> context)
        {
            var inputPath = context.Message.FilePath;
            var outputPath = Path.GetFullPath(
                Path.Combine(Path.GetDirectoryName(inputPath)!, "..", config["OutputVideoPath"]!,
                    context.Message.VideoId.ToString(),
                    $"{Guid.NewGuid().ToString()}.mp4")
            );

            var outputDir = Path.GetDirectoryName(outputPath)!;
            var normalizedPath = Path.Combine(outputDir, "normalized.mp4");
            try
            {
                if (!File.Exists(inputPath))
                {
                    _logger.LogError("Файл {ErrorInput} не був знайдений ", inputPath);
                    throw new Exception("Файл не знайдений");
                }
                
                _logger.LogInformation("Started processing video {inputPath}", inputPath);

                await ValidateVideoAsync(inputPath);

                Directory.CreateDirectory(outputDir);

                var videoInfo = await FFProbe.AnalyseAsync(inputPath);
                var duration = videoInfo.Duration;

                await NormalizeVideoAsync(inputPath, normalizedPath, duration, context.Message.VideoId,
                    context.Message.UserId);
                await GenerateHlsAsync(normalizedPath, outputDir, duration, context.Message.VideoId,
                    context.Message.UserId);
                await GenerateThumbnailAsync(normalizedPath, outputDir);

                await publishEndpoint.Publish(new VideoProcessedEvent
                    { VideoId = context.Message.VideoId, UserId = context.Message.UserId });

                _logger.LogInformation("Video successfully processed");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to convert file: {Error} ", ex.Message);
                await publishEndpoint.Publish(new VideoProcessingFailedEvent(context.Message.VideoId,
                    context.Message.UserId, ex.Message));
                throw;
            }
            finally
            {
                if(File.Exists(inputPath))
                {
                    File.Delete(inputPath);
                }

                if (File.Exists(normalizedPath))
                {
                    File.Delete(normalizedPath);
                }
            }
        }

        private async Task ValidateVideoAsync(string filePath)
        {
            IMediaAnalysis mediaInfo;
            try
            {
                // 2. Probe actual file contents
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
        }

        private async Task NormalizeVideoAsync(string input, string output, TimeSpan duration, Guid videoid,
            Guid userId)
        {
const string filter =
    "split[orig][copy];" +
    "[copy]scale=1080:1920:force_original_aspect_ratio=increase,crop=1080:1920,boxblur=20[bg];" +
    "[orig]scale=1080:1920:force_original_aspect_ratio=decrease[fg];" +
    "[bg][fg]overlay=(W-w)/2:(H-h)/2[v];" +
    "[0:a]loudnorm=I=-16:TP=-1.5:LRA=11[a]";

            await FFMpegArguments
                .FromFileInput(input)
                .OutputToFile(output, overwrite: true, options => options
                    .WithVideoCodec(_opts.Encoding.VideoCodec)
                    .WithAudioCodec(_opts.Encoding.AudioCodec)
                    .WithCustomArgument($"-preset {_opts.Encoding.Preset}")
                    .WithConstantRateFactor(_opts.Encoding.Crf)
                    .WithCustomArgument($"-filter_complex \"{filter}\" -map [v] -map [a]")
                    .WithFastStart())
                .NotifyOnProgress(async void (progress) =>
                {
                    var percent = (int)Math.Floor((progress / duration));
                    await publishEndpoint.Publish(new VideoProcessingProgressEvent(videoid, userId, percent / 2));
                })
                .ProcessAsynchronously();
        }

        private async Task GenerateHlsAsync(string input, string output, TimeSpan duration, Guid videoid, Guid userId)
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
                    .OutputToFile(Path.Combine(dir, "playlist.m3u8"), overwrite: true, options => options
                        .WithVideoCodec(_opts.Encoding.VideoCodec)
                        .WithAudioCodec(_opts.Encoding.AudioCodec)
                        .WithCustomArgument($"-preset {_opts.Encoding.Preset}")
                        .WithCustomArgument($"-vf scale={q.Scale}")
                        .WithCustomArgument($"-b:v {q.VideoBitrate} -maxrate {q.MaxRate} -bufsize {q.BuffSize}")
                        .WithCustomArgument($"-hls_time 4")
                        .WithCustomArgument($"-hls_playlist_type vod")
                        .WithCustomArgument("-hls_flags independent_segments")
                        .WithCustomArgument($"-hls_segment_filename \"{Path.Combine(dir, "seg_%03d.ts")}\"")
                        .ForceFormat("hls"))
                    .NotifyOnProgress(async void (progress) =>
                    {
                        var percent = (int)Math.Floor((progress / duration) * 100);
                        var offset = 50 + (i1 * (50 / qualities.Count));
                        var total = offset + (percent / 2 / qualities.Count);
                        await publishEndpoint.Publish(new VideoProcessingProgressEvent(videoid, userId, total));
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

        private async Task GenerateThumbnailAsync(string input, string output)
        {
            var thumbPath = Path.Combine(output, "thumbnail.jpg");
            await FFMpeg.SnapshotAsync(input, thumbPath, captureTime: TimeSpan.FromSeconds(1));
        }
    }
}*/