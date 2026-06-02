using Application.Interfaces;
using Domain.Entities.Conversation;

namespace Persistence.Repositories.Conversation
{
    internal class ConversationRepository(AppDbContext _context) : GenericRepository<ConversationEntity>(_context), IConversationRepository
    {
    }
}