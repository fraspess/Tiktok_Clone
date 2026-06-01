using Application.Dtos.Message;

namespace Application.Interfaces
{
    public interface IChatNotifier
    {
        Task SendMessageAsync(Guid recipientId, MessageDto message);
        Task SendPendingMessagesAsync(Guid recipientId, IEnumerable<MessageDto> messages);
    }
}