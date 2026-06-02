using Application.Interfaces;
using Domain.Entities.HashTags;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories.HashTag
{
    internal class HashTagRepository(AppDbContext _context) : GenericRepository<HashTagEntity>(_context), IHashTagRepository
    {

        public async Task<HashTagEntity?> GetByNameAsync(string name)
        {
            return await _context.HashTags.FirstOrDefaultAsync(h => h.Tag == name);
        }
    }
}