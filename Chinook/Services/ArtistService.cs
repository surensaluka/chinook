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

        public async Task<List<ArtistModel>> GetArtists()
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return _mapper.Map<List<ArtistModel>>(await dbContext.Artists.Include(a => a.Albums).ToListAsync());
        }

        public async Task<List<ArtistModel>> SearchArtists(string searchText)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return _mapper.Map<List<ArtistModel>>(await dbContext.Artists.Include(a => a.Albums)
            .Where(x => x.Name.ToLower().Contains(searchText.ToLower())).ToListAsync());
        }
    }
}
