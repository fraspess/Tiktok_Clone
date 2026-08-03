namespace VideoProcessor;

public class FfmpegOptions
{
    public List<QualityOptions> Qualities { get; set; } = null!;
    public EncodingOptions Encoding { get; set; } = null!;
}