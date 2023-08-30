using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface ITrackService
    {
        Task<List<PlaylistTrack>> GetTracksByArtistId(long id, string currentUserId);
        Task<PlaylistTrack> ToggleFavoriteTrack(long trackId, bool markAsFavorite, string currentUserId);
        Task<string> AddTrackToPlaylist(long trackId, long playlistId);
        Task<PlaylistTrack> RemoveTrackFromPlaylist(long trackId, long playlistId);
    }
}