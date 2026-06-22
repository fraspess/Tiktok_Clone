namespace Application.Features.Video.Shared;

public class ParsedDescription
{
    public string CleanText { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}