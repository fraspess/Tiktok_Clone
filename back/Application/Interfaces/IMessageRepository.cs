using Domain.Entities.Message;

namespace Application.Interfaces
{
    public interface IMessageRepository : IGenericRepository<MessageEntity>
    {
    }
}