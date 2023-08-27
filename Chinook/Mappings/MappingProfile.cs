using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Utility;

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
                    source => source.Album == null ? "-" : source.Album.Title))

                .ForMember(destination => destination.IsFavorite, opt => opt.MapFrom(
                    (source, destination, _, context) =>
                    {
                        try
                        {
                            if (context.Items[AppConstants.CurrentUserId] == null ||
                            string.IsNullOrEmpty(context.Items[AppConstants.CurrentUserId].ToString()))
                            {
                                return false;
                            }

                            return source.Playlists.Where(p => p.UserPlaylists.Any(
                                            up => up.UserId == context.Items[AppConstants.CurrentUserId].ToString() &&
                                            up.Playlist.Name == AppConstants.Favorites)).Any();
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }));
        }
    }
}