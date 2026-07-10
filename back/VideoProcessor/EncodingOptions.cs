namespace VideoProcessor;

public abstract class EncodingOptions
{
    public string VideoCodec { get; } = string.Empty;
    public string AudioCodec { get; } = string.Empty;
    public string Preset { get; } = string.Empty;
    public int Crf { get; } = 23;
}