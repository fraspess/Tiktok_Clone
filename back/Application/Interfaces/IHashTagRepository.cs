using Domain.Entities.HashTags;

namespace Application.Interfaces
{
    public interface IHashTagRepository : IGenericRepository<HashTagEntity>
    {
        public Task<HashTagEntity?> GetByNameAsync(string name);
    }
}