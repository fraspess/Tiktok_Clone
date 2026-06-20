namespace Application.Features.AdminPanel.GetReports;

public class ReportedContentDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public object? Thumbnail { get; set; }
    public string? ContentUrl { get; set; }
}