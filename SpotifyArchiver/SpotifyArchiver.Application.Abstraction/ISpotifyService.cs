using SpotifyArchiver.Application.Abstraction.dtos;

namespace SpotifyArchiver.Application.Abstraction
{
    public interface ISpotifyService
    {
        public Task<bool> EnsureAuthenticatedAsync(CancellationToken token);
        public Task<IEnumerable<Playlist>> GetPlaylistsAsync(CancellationToken cancellationToken);
    }
}
