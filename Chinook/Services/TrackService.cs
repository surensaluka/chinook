using AutoMapper;
using Chinook.ClientModels;
using Chinook.Utility;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class TrackService : ITrackService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private readonly IMapper _mapper;

        public TrackService(IDbContextFactory<ChinookContext> dbFactory, IMapper mapper)
        {
            _dbFactory = dbFactory;
            _mapper = mapper;
        }

        public async Task<List<PlaylistTrack>> PopulateTracks(long artistId, string currentUserId)
        {
            var DbContext = await _dbFactory.CreateDbContextAsync();

            //return _mapper.Map<List<PlaylistTrack>>(
            //DbContext.Tracks.Where(a => a.Album.ArtistId == artistId)
            //.Include(a => a.Album).ToList(), opts => opts.Items["currentUserId"] = currentUserId);

            return await DbContext.Tracks.Where(a => a.Album.ArtistId == artistId)
                .Include(a => a.Album)
                .Select(t => new PlaylistTrack()
                {
                    AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists
                    .Where(p => p.UserPlaylists
                    .Any(up => up.UserId == currentUserId && up.Playlist.Name == AppConstants.Favorites))
                    .Any()
                }).ToListAsync();
        }

        public async Task<PlaylistTrack> ToggleFavoriteTrack(long trackId, bool markAsFavorite)
        {
            var DbContext = await _dbFactory.CreateDbContextAsync();

            var track = await DbContext.Tracks.Include(a => a.Album).ThenInclude(a => a.Artist)
                .FirstAsync(x => x.TrackId == trackId);

            var favoritePlaylist = await DbContext.Playlists
                .Include(a => a.Tracks)
                .FirstAsync(x => x.Name == AppConstants.Favorites);

            if (markAsFavorite)
                favoritePlaylist.Tracks.Add(track);
            else
                favoritePlaylist.Tracks.Remove(track);

            DbContext.SaveChanges();

            return _mapper.Map<PlaylistTrack>(track);
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

        public async Task<PlaylistTrack> RemoveTrackFromPlaylist(long trackId, long playlistId)
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();

            var playlist = await dbContext.Playlists.Include(x => x.Tracks).FirstAsync(x => x.PlaylistId == playlistId);
            var track = await dbContext.Tracks.FindAsync(trackId);

            if (track != null)
            {
                playlist.Tracks.Remove(track);
                await dbContext.SaveChangesAsync();
            }

            return _mapper.Map<PlaylistTrack>(await dbContext.Tracks.Include(a => a.Album).ThenInclude(a => a.Artist)
                .FirstAsync(x => x.TrackId == trackId));
        }
    }
}