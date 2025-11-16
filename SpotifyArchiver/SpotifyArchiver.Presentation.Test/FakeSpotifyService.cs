using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.Presentation.Test
{
    public class FakeSpotifyService : ISpotifyService
    {
        public bool AuthenticateCalled { get; private set; }
        public bool GetPlaylistsCalled { get; private set; }

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
                new ()
                {
                    Name = "Test Playlist",
                    PlaylistId = 123
                }
            });
        }

        public Task ArchivePlaylist(string playlistId)
        {
            throw new NotImplementedException();
        }
    }
}
