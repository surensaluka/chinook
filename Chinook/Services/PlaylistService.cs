using AutoMapper;
using Chinook.Utility;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private readonly IMapper _mapper;

        public PlaylistService(IDbContextFactory<ChinookContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<List<ClientModels.Playlist>> GetPlaylists()
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return _mapper.Map<List<ClientModels.Playlist>>(await dbContext.Playlists.ToListAsync());
        }

        public async Task<ClientModels.Playlist> GetPlaylistById(long id, string currentUserId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            return await dbContext.Playlists
            .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
            .Where(p => p.PlaylistId == id)
            .Select(p => new ClientModels.Playlist()
            {
                Name = p.Name,
                Tracks = p.Tracks.Select(t => new ClientModels.PlaylistTrack()
                {
                    AlbumTitle = t.Album.Title,
                    ArtistName = t.Album.Artist.Name,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == AppConstants.Favorites)).Any()
                }).ToList()
            }).FirstOrDefaultAsync();

            //_mapper.Map<List<ClientModels.Playlist>>(
        }

        public async Task<ClientModels.Playlist> GetPlaylistByName(string name)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = await dbContext.Playlists
            .Where(p => p.Name == name).FirstOrDefaultAsync();

            return _mapper.Map<ClientModels.Playlist>(playlist);
        }

        public async Task<bool> IsPlaylistNameTaken(string name)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            return await dbContext.Playlists.AnyAsync(x => x.Name != null && x.Name.ToLower()
            .Equals(name.Trim().ToLower()));
        }

        public async Task<long> CreatePlaylist(string name, string currentUserId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            long maxPlaylistId = await dbContext.Playlists.MaxAsync(x => x.PlaylistId);
            await dbContext.Playlists.AddAsync(new Models.Playlist
            {
                Name = name.Trim(),
                PlaylistId = ++maxPlaylistId
            });

            await dbContext.UserPlaylists.AddAsync(new Models.UserPlaylist
            {
                PlaylistId = maxPlaylistId,
                UserId = currentUserId
            });

            await dbContext.SaveChangesAsync();
            return maxPlaylistId;
        }
    }
}
