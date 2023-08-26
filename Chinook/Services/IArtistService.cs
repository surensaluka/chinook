﻿using Chinook.Models;

namespace Chinook.Services
{
    public interface IArtistService
    {
        Task<List<Artist>> GetArtists();
        Task<List<Artist>> SearchArtists(string searchText);
    }
}