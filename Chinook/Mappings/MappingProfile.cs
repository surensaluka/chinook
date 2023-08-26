using AutoMapper;

namespace Chinook.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Playlist, ClientModels.Playlist>();
        }
    }
}
