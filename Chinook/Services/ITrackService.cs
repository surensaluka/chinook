using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface ITrackService
    {
        Task<List<PlaylistTrack>> PopulateTracks(long artistId, string currentUserId);
        Task<PlaylistTrack> ToggleFavoriteTrack(long trackId, bool markAsFavorite);
        Task<string> AddTrackToPlaylist(long trackId, long playlistId);
        Task<PlaylistTrack> RemoveTrackFromPlaylist(long trackId, long playlistId);
    }
}