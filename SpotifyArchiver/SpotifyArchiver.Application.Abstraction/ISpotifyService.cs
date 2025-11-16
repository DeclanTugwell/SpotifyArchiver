using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.Application.Abstraction
{
    public interface ISpotifyService
    {
        public Task<bool> TryAuthenticateAsync(CancellationToken token);
        public Task<List<Playlist>> GetPlaylistsAsync();
        public Task ArchivePlaylist(string playlistId);
    }
}
