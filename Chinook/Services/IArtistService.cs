using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IArtistService
    {
        Task<Artist> GetArtistById(long id);
        Task<List<Artist>> GetArtists();
        Task<List<Artist>> SearchArtists(string searchText);
    }
}