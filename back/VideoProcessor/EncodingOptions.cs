namespace VideoProcessor;

public class EncodingOptions
{
    public string VideoCodec { get; set; }
    public string AudioCodec { get; set; }
    public string Preset { get; set; }
    public int Crf { get; set; }
}