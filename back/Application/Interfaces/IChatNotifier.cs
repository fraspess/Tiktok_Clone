using Application.Dtos.Message;

namespace Application.Interfaces;

public interface IChatNotifier
{
    Task SendReceiptAsync(Guid recipientId, Guid conversationId, Guid messageId, bool isDelivered, bool isRead);
    Task SendMessageAsync(Guid recipientId, MessageDto message);
    Task SendPendingMessagesAsync(Guid recipientId, IEnumerable<MessageDto> messages);
}
