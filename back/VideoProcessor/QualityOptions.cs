namespace VideoProcessor;

public class QualityOptions
{
    public int Quality { get; set; }
    public string Scale { get; set; } = string.Empty;
    public string VideoBitrate { get; set; } = string.Empty;
    public string MaxRate { get; set; } = string.Empty;
    public string BuffSize { get; set; } = string.Empty;
    public string AudioBitrate { get; set; } = string.Empty;
    public int Bandwidth { get; set; }
}