using SpotifyAPI.Web;
using SpotifyArchiver.Application.Abstraction;

namespace SpotifyArchiver.Application.Implementation;

public class SpotifyService : ISpotifyService
{
    private readonly ISpotifyAuthService _spotifyAuthService;

    public SpotifyService(ISpotifyAuthService spotifyAuthService)
    {
        _spotifyAuthService = spotifyAuthService;
    }

    public async Task GetUserPlaylists()
    {
        var spotifyClient = await _spotifyAuthService.GetAuthenticatedClient();
        var playlists = await spotifyClient.Playlists.CurrentUsers();
        await foreach (var playlist in spotifyClient.Paginate(playlists))
        {
            Console.WriteLine(playlist.Name);
        }
    }
}