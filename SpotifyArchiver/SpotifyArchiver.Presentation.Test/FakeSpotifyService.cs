using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.Presentation.Test
{
    public class FakeSpotifyService : ISpotifyService
    {
        public bool AuthenticateCalled { get; private set; }
        public bool GetPlaylistsCalled { get; private set; }
        public bool GetArchivedPlaylistsCalled { get; private set; }

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
                    PlaylistId = 123,
                    SpotifyId = "test_playlist_id",
                    SpotifyUri = "spotify:playlist:test_playlist_id"
                }
            });
        }

        public Task ArchivePlaylist(string playlistId)
        {
            return Task.CompletedTask;
        }

        public Task<List<Playlist>> GetArchivedPlaylistsAsync()
        {
            GetArchivedPlaylistsCalled = true;
            return Task.FromResult(new List<Playlist>
            {
                new()
                {
                    PlaylistId = 1,
                    Name = "Archived Playlist",
                    SpotifyId = "archived_id",
                    SpotifyUri = "spotify:playlist:archived_id"
                }
            });
        }
    }
}
