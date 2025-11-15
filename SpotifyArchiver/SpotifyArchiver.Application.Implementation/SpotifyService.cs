using SpotifyAPI.Web;
using SpotifyArchiver.Application.Abstraction;


namespace SpotifyArchiver.Application.Implementation
{
    public class SpotifyService : ISpotifyService
    {
        private readonly SpotifyAuthService _authService;
        private SpotifyClient? _client;

        public SpotifyService(string clientId, string redirectUri, string configPath)
        {
            _authService = new SpotifyAuthService(clientId, redirectUri, configPath);
        }

        public async Task<bool> EnsureAuthenticatedAsync(CancellationToken token)
        {
            _client??= await _authService.FetchAuthenticatedClient(token);

            return _client != null;
        }
    }
}
