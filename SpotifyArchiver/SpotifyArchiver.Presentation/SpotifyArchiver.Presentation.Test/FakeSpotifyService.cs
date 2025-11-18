using SpotifyArchiver.Application.Abstraction;

namespace SpotifyArchiver.Presentation.Test
{
    public class FakeSpotifyService : ISpotifyService
    {
        public bool AuthenticateCalled { get; private set; }
        public bool GetPlaylistsCalled { get; private set; }
        public bool ArchivePlaylistCalled { get; private set; }
        public string? ArchivedPlaylistId { get; private set; }

        public Task<bool> TryAuthenticateAsync(CancellationToken token)
        {
            AuthenticateCalled = true;
            return Task.FromResult(true);
        }

        public Task<List<Playlist>> GetPlaylistsAsync()
        {
            GetPlaylistsCalled = true;
            return Task.FromResult(new List<Playlist>
            {
                new ("123", "Test Playlist", 42)
            });
        }

        public Task<Playlist> GetPlaylistAsync(string playlistId)
        {
            throw new NotImplementedException();
        }

        public Task ArchivePlaylistAsync(string playlistId)
        {
            ArchivePlaylistCalled = true;
            ArchivedPlaylistId = playlistId;
            return Task.CompletedTask;
        }
    }
}
