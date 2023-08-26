using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IPlaylistService
    {
        Task<List<Playlist>> GetAllPlaylists();
        Task<bool> IsPlaylistNameTaken(string playlistName);
        Task<long> CreatePlaylist(string playlistName, string userId);
        Task<string> AddTrackToPlaylist(long trackId, long playlistId);
    }
}