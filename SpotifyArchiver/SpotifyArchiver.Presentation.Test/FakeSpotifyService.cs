using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Domain;

namespace SpotifyArchiver.Presentation.Test
{
    public class FakeSpotifyService : ISpotifyService
    {
        public bool GetPlaylistsCalled { get; private set; }

        public Task<List<Playlist>> GetPlaylistsAsync()
        {
            GetPlaylistsCalled = true;
            return Task.FromResult(new List<Playlist>
            {
                new Playlist { SpotifyId = "1", Name = "Test Playlist" }
            });
        }

        public Task<List<Track>> GetPlaylistTracksAsync(string playlistId)
        {
            return Task.FromResult(new List<Track>());
        }

        public Task<bool> TryAuthenticateAsync(CancellationToken token)
        {
            return Task.FromResult(true);
        }
    }
}
