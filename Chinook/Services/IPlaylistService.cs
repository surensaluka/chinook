using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IPlaylistService
    {
        Task<List<Playlist>> GetAllPlaylists();
        Task<Playlist> GetPlaylistById(long PlaylistId, string currentUserId);
        Task<Playlist> GetPlaylistByName(string PlaylistName);
        Task<bool> IsPlaylistNameTaken(string playlistName);
        Task<long> CreatePlaylist(string playlistName, string userId);
    }
}