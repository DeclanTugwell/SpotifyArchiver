using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotifyArchiver.Application.Abstraction
{
    public interface ISpotifyService
    {
        Task<List<FullPlaylist>> GetCurrentUserPlaylists();
    }
}
