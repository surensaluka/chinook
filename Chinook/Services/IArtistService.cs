using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IArtistService
    {
        Task<List<ArtistModel>> GetArtists();
        Task<List<ArtistModel>> SearchArtists(string searchText);
    }
}