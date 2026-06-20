namespace Application.Services.Message;

public interface IMessageService
{
    public Task SendAsync(Guid userId, Guid conversationId, string content);
    public Task FlushPendingAsync(Guid userId);
    public Task MarkAsReadAsync(Guid userId, Guid messageId);
    public Task MarkAsDeliveredAsync(Guid userId, Guid messageId);
}