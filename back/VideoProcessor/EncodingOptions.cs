namespace VideoProcessor;

public class EncodingOptions
{
    public string VideoCodec { get; set; } = string.Empty;
    public string AudioCodec { get; set; } = string.Empty;
    public string Preset { get; set; } = string.Empty;
    public int Crf { get; set; } = 23;
}