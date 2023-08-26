using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;

        public ArtistService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<Artist>> GetArtists()
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return dbContext.Artists.Include(a => a.Albums).ToList();
        }

        public async Task<List<Artist>> SearchArtists(string searchText)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return await dbContext.Artists.Include(a => a.Albums)
            .Where(x => x.Name.ToLower().Contains(searchText.ToLower())).ToListAsync();
        }
    }
}
