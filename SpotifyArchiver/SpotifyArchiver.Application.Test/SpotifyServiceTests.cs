using Shouldly;
using SpotifyArchiver.Application.Implementation;

namespace SpotifyArchiver.Application.Test
{
    public class SpotifyServiceTests
    {
        private readonly string _clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not set");
        private readonly string _redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI") ?? throw new InvalidOperationException("SPOTIFY_REDIRECT_URI not set");
        private readonly string _configPath = "spotify_tokens_test.json";

        [Test]
        [Explicit("Requires SPOTIFY_CLIENT_ID and SPOTIFY_REDIRECT_URI environment variables to be set based on setup within Spotify Developer Portal")]
        [Category("Spotify Integration")]
        public async Task Test_AuthenticationFlow()
        {
            var service = new SpotifyService(_clientId, _redirectUri, _configPath, new FakePlaylistRepository());
            var authenticated = await service.TryAuthenticateAsync(CancellationToken.None);
            authenticated.ShouldBeTrue();
            File.Exists(_configPath).ShouldBeTrue();
        }

        [Test]
        [Explicit("Requires SPOTIFY_CLIENT_ID and SPOTIFY_REDIRECT_URI environment variables to be set based on setup within Spotify Developer Portal")]
        [Category("Spotify Integration")]
        public async Task Test_WhenGetPlaylistsAsyncCalled_ThenPlaylistsReturned()
        {
            var service = new SpotifyService(_clientId, _redirectUri, _configPath, new FakePlaylistRepository());
            (await service.TryAuthenticateAsync(CancellationToken.None)).ShouldBeTrue();
            var playlists = await service.GetPlaylistsAsync();
            playlists.ShouldNotBeEmpty();
        }

        [Test]
        [Explicit("Requires SPOTIFY_CLIENT_ID and SPOTIFY_REDIRECT_URI environment variables to be set based on setup within Spotify Developer Portal")]
        [Category("Spotify Integration")]
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
        [Explicit("Requires SPOTIFY_CLIENT_ID and SPOTIFY_REDIRECT_URI environment variables to be set based on setup within Spotify Developer Portal")]
        [Category("Spotify Integration")]
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

        [Test]
        public async Task Test_WhenGetArchivedPlaylistsAsync_ThenAllArchivedPlaylistsReturned()
        {
            // Arrange
            var repo = new FakePlaylistRepository();
            await repo.AddAsync(new DataAccess.Abstraction.entities.Playlist
            {
                SpotifyId = "test",
                Name = "test",
                SpotifyUri = "test"
            });
            var service = new SpotifyService(_clientId, _redirectUri, _configPath, repo);

            // Act
            var result = await service.GetArchivedPlaylistsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
        }
    }
}
