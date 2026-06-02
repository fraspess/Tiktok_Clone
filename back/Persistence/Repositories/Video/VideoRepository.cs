using Application.Interfaces;
using Domain.Entities.Video;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories.Video
{
    internal class VideoRepository(AppDbContext _context) : GenericRepository<VideoEntity>(_context), IVideoRepository
    {

        public async Task<VideoEntity?> FindVideoBySomeString(string someString)
        {
            return await _context.Videos
                .Include(v => v.HashTags)
                .Include(v => v.Author)
                .FirstOrDefaultAsync(v =>
                    v.HashTags.Any(h => h.HashTag.Tag.StartsWith(someString)) || v.Author!.UserName == someString);
        }
    }
}