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

        public Task<List<Track>> GetArchivedSongs(int playlistDbId)
        {
            return Task.FromResult(new List<Track>
            {
                new() { Name = "Test Song 1", ArtistName = "Test Artist 1", SpotifyId = "spotify:track:1", SpotifyUri = "uri1" },
                new() { Name = "Test Song 2", ArtistName = "Test Artist 2", SpotifyId = "spotify:track:2", SpotifyUri = "uri2" },
            });
        }
    }
}
