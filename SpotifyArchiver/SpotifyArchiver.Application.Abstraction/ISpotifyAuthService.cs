using SpotifyAPI.Web;

namespace SpotifyArchiver.Application.Abstraction;

public interface ISpotifyAuthService
{
    Task<ISpotifyClient> GetAuthenticatedClient();
}