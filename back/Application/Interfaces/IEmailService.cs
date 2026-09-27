namespace Application.Interfaces;

public interface IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body);
}