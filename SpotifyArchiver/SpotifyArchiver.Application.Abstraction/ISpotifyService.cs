namespace SpotifyArchiver.Application.Abstraction;

public interface ISpotifyService
{
    Task GetUserPlaylists();
}