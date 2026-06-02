using Application.Interfaces;
using Domain.Entities.Message;

namespace Persistence.Repositories.Message
{
    internal class MessageRepository(AppDbContext _context) : GenericRepository<MessageEntity>(_context), IMessageRepository
    {
    }
}