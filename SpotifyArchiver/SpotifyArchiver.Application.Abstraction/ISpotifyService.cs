using SpotifyArchiver.Domain;

namespace SpotifyArchiver.Application.Abstraction
{
    public interface ISpotifyService
    {
        public Task<bool> TryAuthenticateAsync(CancellationToken token);
        public Task<List<Playlist>> GetPlaylistsAsync();
        public Task<List<Track>> GetPlaylistTracksAsync(string playlistId);
    }
}
