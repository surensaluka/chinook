using AutoMapper;
using Chinook.Utility;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private readonly IMapper _mapper;
        private readonly IEventService _eventService;

        public PlaylistService(IDbContextFactory<ChinookContext> dbFactory, IMapper mapper,
            IEventService eventService)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
            _eventService = eventService;
        }

        public async Task<List<ClientModels.Playlist>> GetPlaylists(string currentUserId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return _mapper.Map<List<ClientModels.Playlist>>(
                await dbContext.Playlists.Include(p => p.UserPlaylists)
                .Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId)).ToListAsync());
        }

        public async Task<ClientModels.Playlist> GetPlaylistById(long id, string currentUserId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            return _mapper.Map<Models.Playlist, ClientModels.Playlist>(await dbContext.Playlists
                .Include(p => p.Tracks).ThenInclude(t => t.Album).ThenInclude(a => a.Artist)
                .Include(p => p.Tracks).ThenInclude(t => t.Playlists).ThenInclude(p => p.UserPlaylists)
                .Where(p => p.PlaylistId == id)
            .FirstOrDefaultAsync(), opt => opt.Items[AppConstants.CurrentUserId] = currentUserId);
        }

        public async Task<ClientModels.Playlist> GetPlaylistByName(string name)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = await dbContext.Playlists
            .Where(a => a.Name == name).FirstOrDefaultAsync();

            return _mapper.Map<ClientModels.Playlist>(playlist);
        }


        public async Task<long> CreatePlaylist(string name, string currentUserId)
        {
            if (await IsPlaylistNameTaken(name))
            {
                throw new ArgumentException();
            }

            var dbContext = await _dbFactory.CreateDbContextAsync();

            long maxPlaylistId = 0;
            try
            {
                maxPlaylistId = await dbContext.Playlists.MaxAsync(p => p.PlaylistId);
            }
            catch (Exception) { }

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
            _eventService.SendRefreshPlaylistEvent();
            return maxPlaylistId;
        }

        private async Task<bool> IsPlaylistNameTaken(string name)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            return await dbContext.Playlists.AnyAsync(p => p.Name != null && p.Name.ToLower()
            .Equals(name.Trim().ToLower()));
        }
    }
}
