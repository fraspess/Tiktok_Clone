using Microsoft.Extensions.Options;

namespace VideoProcessor;

public class LocalVideoStorage(IOptions<LocalStorageOptions> options) : IVideoFileStorage
{
    private readonly LocalStorageOptions _options = options.Value;

    public Task DownloadOriginalAsync(Guid videoId, string dest)
    {
        var sourcePath = Path.Combine(_options.RootPath, "uploads", "unprocessed", videoId.ToString(), "original");

        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Оригінальний файл не знайдений", sourcePath);

        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
        File.Copy(sourcePath, dest, overwrite: true);

        return Task.CompletedTask;
    }

    public Task UploadProcessedAsync(Guid videoId, string source)
    {
        var destDir = Path.Combine(_options.RootPath, "uploads", "processed", videoId.ToString());
        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(source, file);
            var destPath = Path.Combine(destDir, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
            File.Copy(file, destPath, overwrite: true);
        }

        return Task.CompletedTask;
    }
}