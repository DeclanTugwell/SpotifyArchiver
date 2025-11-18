using System.Collections.Generic;
using SpotifyArchiver.Application.Abstraction.Dtos;

namespace SpotifyArchiver.Application.Abstraction
{
    public interface ISpotifyService
    {
        public Task<bool> EnsureAuthenticatedAsync(CancellationToken token);
        public Task<List<PlaylistDTO>> GetPlaylistsAsync();
    }
}
