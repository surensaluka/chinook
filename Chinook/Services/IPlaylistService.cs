using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IPlaylistService
    {
        Task<List<Playlist>> GetPlaylists();
        Task<Playlist> GetPlaylistById(long id, string currentUserId);
        Task<Playlist> GetPlaylistByName(string name);
        Task<bool> IsPlaylistNameTaken(string name);
        Task<long> CreatePlaylist(string name, string currentUserId);
    }
}