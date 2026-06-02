using Domain.Entities.Identity;

namespace Domain.Entities.Conversation
{
    public class ConversationParticipant
    {
        public Guid UserId { get; set; }

        public DateTime? LastReadAt { get; set; }

        public Guid ConversationId { get; set; }
        public ConversationEntity Conversation { get; set; } = null!;
        public UserEntity User { get; set; } = null!;
    }
}