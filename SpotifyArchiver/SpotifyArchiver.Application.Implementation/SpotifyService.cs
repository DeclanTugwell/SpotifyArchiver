using SpotifyAPI.Web;
using SpotifyArchiver.Application.Abstraction;

namespace SpotifyArchiver.Application.Implementation;

public class SpotifyService : ISpotifyService
{
    private readonly ISpotifyAuthService _authService;

    public SpotifyService(ISpotifyAuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<FullPlaylist>> GetCurrentUserPlaylists()
    {
        var spotify = await _authService.GetAuthenticatedClient();
        // TODO: Implement actual logic to fetch playlists
        throw new NotImplementedException();
    }
}
