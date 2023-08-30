using AutoMapper;
using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private readonly IMapper _mapper;

        public ArtistService(IDbContextFactory<ChinookContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<Artist> GetArtistById(long id)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return _mapper.Map<Artist>(dbContext.Artists.SingleOrDefault(a => a.ArtistId == id));
        }

        public async Task<List<Artist>> GetArtists()
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return _mapper.Map<List<Artist>>(await dbContext.Artists.Include(a => a.Albums).ToListAsync());
        }

        public async Task<List<Artist>> SearchArtists(string searchText)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return _mapper.Map<List<Artist>>(await dbContext.Artists.Include(a => a.Albums)
            .Where(a => a.Name != null && a.Name.ToLower().Contains(searchText.ToLower())).ToListAsync());
        }
    }
}
