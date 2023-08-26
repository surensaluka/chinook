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
            CreateMap<Models.Artist, ClientModels.Artist>();


            CreateMap<Track, PlaylistTrack>()
                .ForMember(destination => destination.TrackName, opt => opt.MapFrom(source => source.Name))
                .ForMember(destination => destination.ArtistName, opt => opt.MapFrom(
                    source => source.Album.Artist.Name))
                .ForMember(destination => destination.AlbumTitle, opt => opt.MapFrom(
                    source => source.Album == null ? "-" : source.Album.Title));

            //.ForMember(destination => destination.IsFavorite, opt => opt.MapFrom(
            //    (source, destination, destinationMember, context) =>
            //    {
            //        return source.Playlists.Where(p => p.UserPlaylists.Any(
            //            up => up.UserId == context.Items["currentUserId"].ToString() &&
            //            up.Playlist.Name == AppConstants.Favorites)).Any();
            //    }));


            CreateMap<Models.Playlist, ClientModels.Playlist>();

        }
    }
}