using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;

namespace VideoProcessor;

internal class S3VideoFileStorage(IAmazonS3 amazonS3, IOptions<AwsS3Options> awsS3Options) : IVideoFileStorage
{
    private AwsS3Options _options = awsS3Options.Value;

    public async Task DownloadOriginalAsync(Guid videoId, string dest)
    {
        using var transferUtility = new TransferUtility(amazonS3);
        await transferUtility.DownloadAsync(dest, _options.BucketName, $"uploads/unprocessed/{videoId}/original");
    }

    public async Task UploadProcessedAsync(Guid videoId, string source)
    {
        using var transferUtility = new TransferUtility(amazonS3);
        await transferUtility.UploadDirectoryAsync(new TransferUtilityUploadDirectoryRequest
        {
            BucketName = _options.BucketName,
            Directory = source,
            KeyPrefix = $"uploads/processed/{videoId}",
            SearchPattern = "*",
            SearchOption = SearchOption.AllDirectories
        });
    }
}