using System.ComponentModel.DataAnnotations;
using Domain.Entities.Conversation;
using Domain.Entities.Identity;

namespace Domain.Entities.Message
{
    public class MessageEntity : BannableSoftDeletableEntity
    {

        public required Guid SenderId { get; set; }
        public UserEntity Sender { get; set; } = null!;
        public bool IsDelivered { get; set; } = false;
        
        [MaxLength(255)]

        public required string Content { get; set; }

        public bool IsRead { get; set; } = false;

        public Guid ConversationId { get; set; }

        public ConversationEntity Conversation { get; set; } = null!;
    }
}