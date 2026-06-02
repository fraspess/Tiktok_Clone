using Domain.Entities.Message;

namespace Domain.Entities.Conversation
{
    public class ConversationEntity : AuditableEntity
    {
        public ICollection<ConversationParticipant> Participants { get; set; } = [];
        public ICollection<MessageEntity> Messages { get; set; } = [];
    }
}