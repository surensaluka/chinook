using AutoMapper;
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

        public async Task<List<ClientModels.Playlist>> GetAllPlaylists()
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return _mapper.Map<List<ClientModels.Playlist>>(await dbContext.Playlists.ToListAsync());
        }

        public async Task<bool> IsPlaylistNameTaken(string playlistName)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            return await dbContext.Playlists.AnyAsync(x => x.Name != null && x.Name.ToLower()
            .Equals(playlistName.Trim().ToLower()));
        }

        public async Task<long> CreatePlaylist(string playlistName, string userId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            long maxPlaylistId = await dbContext.Playlists.MaxAsync(x => x.PlaylistId);
            await dbContext.Playlists.AddAsync(new Models.Playlist
            {
                Name = playlistName.Trim(),
                PlaylistId = ++maxPlaylistId
            });

            await dbContext.UserPlaylists.AddAsync(new Models.UserPlaylist
            {
                PlaylistId = maxPlaylistId,
                UserId = userId
            });

            await dbContext.SaveChangesAsync();
            return maxPlaylistId;
        }

        public async Task<string> AddTrackToPlaylist(long trackId, long playlistId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = await dbContext.Playlists.Include(x => x.Tracks).FirstAsync(x => x.PlaylistId == playlistId);
            var track = await dbContext.Tracks.FindAsync(trackId);

            if (playlist != null && track != null && !playlist.Tracks.Any(f => f.TrackId == track.TrackId))
            {
                playlist.Tracks.Add(track);
                await dbContext.SaveChangesAsync();
                return playlist.Name;
            }

            return (playlist == null) ? string.Empty : playlist.Name;
        }
    }
}
