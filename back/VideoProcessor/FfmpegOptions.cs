namespace VideoProcessor;

public class FfmpegOptions
{
    public List<QualityOptions> Qualities { get; } = null!;
    public EncodingOptions Encoding { get; } = null!;
}