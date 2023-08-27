using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Utility;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class TrackService : ITrackService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private readonly IMapper _mapper;
        private readonly IPlaylistService _playlistService;

        public TrackService(IDbContextFactory<ChinookContext> dbFactory, IMapper mapper, IPlaylistService playlistService)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
            _playlistService = playlistService;
        }

        public async Task<List<PlaylistTrack>> GetTracksByArtistId(long id, string currentUserId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            return _mapper.Map<List<Track>, List<PlaylistTrack>>(
            dbContext.Tracks.Include(t => t.Playlists)
            .ThenInclude(p => p.UserPlaylists)
            .Include(t => t.Album).Where(t => t.Album.ArtistId == id).ToList(),
            opts => opts.Items[AppConstants.CurrentUserId] = currentUserId);
        }

        public async Task<PlaylistTrack> ToggleFavoriteTrack(long trackId, bool markAsFavorite, string currentUserId)
        {
            await CreatePlaylistIfNotExists(currentUserId);

            var dbContext = await _dbFactory.CreateDbContextAsync();

            var favoritePlaylist = await dbContext.Playlists
                .Include(p => p.Tracks)
                .FirstAsync(p => p.Name == AppConstants.Favorites);

            var track = await dbContext.Tracks.Include(t => t.Album)
                .ThenInclude(a => a.Artist)
                .FirstAsync(t => t.TrackId == trackId);

            if (markAsFavorite)
                favoritePlaylist.Tracks.Add(track);
            else
                favoritePlaylist.Tracks.Remove(track);

            await dbContext.SaveChangesAsync();

            return _mapper.Map<Track, PlaylistTrack>(track, opt => opt.Items[AppConstants.CurrentUserId] = string.Empty);
        }

        private async Task CreatePlaylistIfNotExists(string currentUserId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var favoritePlaylist = await dbContext.Playlists
               .FirstOrDefaultAsync(p => p.Name == AppConstants.Favorites);

            if (favoritePlaylist == null)
            {
                await _playlistService.CreatePlaylist(AppConstants.Favorites, currentUserId);
            }
        }

        public async Task<string> AddTrackToPlaylist(long trackId, long playlistId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = await dbContext.Playlists.Include(p => p.Tracks).FirstAsync(p => p.PlaylistId == playlistId);
            var track = await dbContext.Tracks.FindAsync(trackId);

            if (playlist != null && track != null && !playlist.Tracks.Any(t => t.TrackId == track.TrackId))
            {
                playlist.Tracks.Add(track);
                await dbContext.SaveChangesAsync();
                return playlist.Name;
            }

            return (playlist == null) ? string.Empty : playlist.Name;
        }

        public async Task<PlaylistTrack> RemoveTrackFromPlaylist(long trackId, long playlistId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = await dbContext.Playlists.Include(a => a.Tracks).FirstAsync(p => p.PlaylistId == playlistId);
            var track = await dbContext.Tracks.FindAsync(trackId);

            if (track != null)
            {
                playlist.Tracks.Remove(track);
                await dbContext.SaveChangesAsync();
            }

            return _mapper.Map<PlaylistTrack>(await dbContext.Tracks.Include(t => t.Album).ThenInclude(a => a.Artist)
                .FirstAsync(t => t.TrackId == trackId));
        }
    }
}