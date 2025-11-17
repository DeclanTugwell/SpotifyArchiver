using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace SpotifyArchiver.Application.Abstraction
{
    public interface ISpotifyAuthService
    {
        Task<ISpotifyClient> GetAuthenticatedClient();
    }
}
