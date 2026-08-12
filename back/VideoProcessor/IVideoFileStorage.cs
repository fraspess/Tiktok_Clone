namespace VideoProcessor;

public interface IVideoFileStorage
{
    Task DownloadOriginalAsync(Guid videoId, string dest);
    Task UploadProcessedAsync(Guid videoId, string source);
}