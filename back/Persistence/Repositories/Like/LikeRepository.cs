using Application.Interfaces;
using Domain.Entities.Like;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories.Like;

internal class LikeRepository(AppDbContext _context) : GenericRepository<LikeEntity>(_context), ILikeRepository
{

    public async Task<LikeEntity?> GetLikeByUserAndVideoIdAsync(Guid userId, Guid videoId)
    {
        return await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.VideoId == videoId);
    }
}