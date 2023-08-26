using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface ITrackService
    {
        Task<List<PlaylistTrack>> PopulateTracks(long artistId, string currentUserId);
        Task<PlaylistTrack> ToggleFavoriteTrack(long trackId, bool markAsFavorite);
        //Task<PlaylistTrack> UnfavoriteTrack(long trackId);
    }
}