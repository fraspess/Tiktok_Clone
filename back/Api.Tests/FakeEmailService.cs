using System.Collections.Concurrent;
using Application.Interfaces;

namespace Api.Tests;

public record SentEmail(string To, string Subject, string Body);

public sealed class FakeEmailService : IEmailService
{
    private readonly ConcurrentQueue<SentEmail> _sent = new();
    public IReadOnlyCollection<SentEmail> SentEmails => _sent.ToArray();

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _sent.Enqueue(new SentEmail(to, subject, body));
        return Task.CompletedTask;
    }
}