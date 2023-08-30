using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IPlaylistService
    {
        Task<List<Playlist>> GetPlaylists(string currentUserId);
        Task<Playlist> GetPlaylistById(long id, string currentUserId);
        Task<Playlist> GetPlaylistByName(string name);
        Task<long> CreatePlaylist(string name, string currentUserId);
    }
}