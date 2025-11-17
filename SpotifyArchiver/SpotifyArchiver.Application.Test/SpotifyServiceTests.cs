using Shouldly;
using SpotifyArchiver.Application.Implementation;

namespace SpotifyArchiver.Application.Test
{
    [Explicit("Requires SPOTIFY_CLIENT_ID and SPOTIFY_REDIRECT_URI environment variables to be set based on setup within Spotify Developer Portal")]
    [Category("Spotify Integration")]
    public class SpotifyServiceTests
    {
        private readonly string _clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not set");
        private readonly string _redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI") ?? throw new InvalidOperationException("SPOTIFY_REDIRECT_URI not set");
        private readonly string _configPath = "spotify_tokens_test.json";

        [Test]
        public async Task Test_AuthenticationFlow()
        {
            var service = new SpotifyService(_clientId, _redirectUri, _configPath, new FakePlaylistRepository());
            var authenticated = await service.TryAuthenticateAsync(CancellationToken.None);
            authenticated.ShouldBeTrue();
            File.Exists(_configPath).ShouldBeTrue();
        }

        [Test]
        public async Task Test_WhenGetPlaylistsAsyncCalled_ThenPlaylistsReturned()
        {
            var service = new SpotifyService(_clientId, _redirectUri, _configPath, new FakePlaylistRepository());
            (await service.TryAuthenticateAsync(CancellationToken.None)).ShouldBeTrue();
            var playlists = await service.GetPlaylistsAsync();
            playlists.ShouldNotBeEmpty();
        }

        [Test]
        public async Task Test_WhenArchivePlaylistCalled_WithValidPlaylistId_ThenPlaylistsArchived()
        {
            var repo = new FakePlaylistRepository();
            var service = new SpotifyService(_clientId, _redirectUri, _configPath, repo);
            (await service.TryAuthenticateAsync(CancellationToken.None)).ShouldBeTrue();
            var playlists = await service.GetPlaylistsAsync();
            playlists.ShouldNotBeEmpty();
            var firstPlaylist = playlists.First();
            await service.ArchivePlaylist(firstPlaylist.SpotifyId);
            repo.ArchivedPlaylists.ShouldContain(p => p.SpotifyId == firstPlaylist.SpotifyId);
        }

        [Test]
        public async Task Test_WhenArchivePlaylistCalled_WithInvalidPlaylistId_ThenPlaylistsArchived()
        {
            var exceptionThrown = false;
            var service = new SpotifyService(_clientId, _redirectUri, _configPath, new FakePlaylistRepository());
            (await service.TryAuthenticateAsync(CancellationToken.None)).ShouldBeTrue();
            try
            {
                await service.ArchivePlaylist("");
            }
            catch
            {
                exceptionThrown = true;
            }
            exceptionThrown.ShouldBeTrue();
        }
    }

    [Category("Unit")]
    public class SpotifyServiceUnitTests
    {
        [Test]
        public async Task Test_GetArchivedSongs_ShouldReturnSongs_WhenPlaylistExists()
        {
            // Arrange
            var repo = new FakePlaylistRepository();
            var playlistDbId = 1;
            var tracks = new List<DataAccess.Abstraction.entities.Track>
            {
                new() { Name = "Song 1", ArtistName = "Artist 1", SpotifyId = "spotify:track:1", SpotifyUri = "uri1" },
                new() { Name = "Song 2", ArtistName = "Artist 2", SpotifyId = "spotify:track:2", SpotifyUri = "uri2" },
            };
            
            await repo.AddAsync(new DataAccess.Abstraction.entities.Playlist
            {
                PlaylistId = playlistDbId,
                SpotifyId = "test_playlist",
                Name = "Test Playlist",
                SpotifyUri = "uri",
                Tracks = tracks
            });
            
            var service = new SpotifyService("client_id", "redirect_uri", "config.json", repo);

            // Act
            var result = await service.GetArchivedSongs(playlistDbId);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result[0].Name.ShouldBe("Song 1");
            result[1].Name.ShouldBe("Song 2");
        }
        
        [Test]
        public async Task Test_GetArchivedSongs_ShouldThrowException_WhenPlaylistDoesNotExist()
        {
            // Arrange
            var repo = new FakePlaylistRepository();
            var service = new SpotifyService("client_id", "redirect_uri", "config.json", repo);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(async () => await service.GetArchivedSongs(999));
        }
    }
}
