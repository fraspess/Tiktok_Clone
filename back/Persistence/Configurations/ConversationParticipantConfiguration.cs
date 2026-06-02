using Domain.Entities.Conversation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        
        builder
            .HasKey(p => new { p.ConversationId, p.UserId });
        
        builder
            .HasOne(p => p.User)
            .WithMany(u => u.ConversationParticipants)
            .HasForeignKey(p => p.UserId);

        builder
            .HasOne(p => p.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(p => p.ConversationId);
    }
}