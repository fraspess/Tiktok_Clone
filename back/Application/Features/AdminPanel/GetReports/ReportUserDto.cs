namespace Application.Features.AdminPanel.GetReports;

public class ReportUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public object? Avatar { get; set; }
}