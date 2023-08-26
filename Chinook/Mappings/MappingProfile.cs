using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Playlist, ClientModels.Playlist>();
            CreateMap<Artist, ArtistModel>();

        }
    }
}
