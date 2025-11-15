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
            var service = new SpotifyService(_clientId, _redirectUri, _configPath);
            var authenticated = await service.TryAuthenticateAsync(CancellationToken.None);
            authenticated.ShouldBeTrue();
            File.Exists(_configPath).ShouldBeTrue();
        }

        [Test]
        public async Task Test_WhenGetPlaylistsAsyncCalled_ThenPlaylistsReturned()
        {
            var service = new SpotifyService(_clientId, _redirectUri, _configPath);
            (await service.TryAuthenticateAsync(CancellationToken.None)).ShouldBeTrue();
            var playlists = await service.GetPlaylistsAsync();
            playlists.ShouldNotBeEmpty();
        }
    }
}
