using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IArtistService
    {
        Task<Artist> GetArtistById(long artistId);
        Task<List<Artist>> GetArtists();
        Task<List<Artist>> SearchArtists(string searchText);
    }
}