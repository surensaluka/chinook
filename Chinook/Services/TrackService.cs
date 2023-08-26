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

            return DbContext.Tracks.Where(a => a.Album.ArtistId == artistId)
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
                }).ToList();
        }

        public async Task<PlaylistTrack> ToggleFavoriteTrack(long trackId, bool markAsFavorite)
        {
            var DbContext = await _dbFactory.CreateDbContextAsync();
            var track = DbContext.Tracks.Include(a => a.Album).ThenInclude(a => a.Artist)
                .First(x => x.TrackId == trackId);
            var favoritePlaylist = DbContext.Playlists
                .Include(a => a.Tracks)
                .First(x => x.Name == AppConstants.Favorites);

            if (markAsFavorite)
                favoritePlaylist.Tracks.Add(track);
            else
                favoritePlaylist.Tracks.Remove(track);

            DbContext.SaveChanges();

            return _mapper.Map<PlaylistTrack>(track);
        }
    }
}