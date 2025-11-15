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
        public async Task TestAuthenticationFlow()
        {
            var service = new SpotifyService(_clientId, _redirectUri, _configPath);
            var authenticated = await service.EnsureAuthenticatedAsync(CancellationToken.None);
            authenticated.ShouldBeTrue();
            File.Exists(_configPath).ShouldBeTrue();
        }
    }
}
