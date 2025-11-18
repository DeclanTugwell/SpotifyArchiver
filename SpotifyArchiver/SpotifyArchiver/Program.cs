using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation;

var authService = new SpotifyAuthService();
ISpotifyService spotifyService = new SpotifyService(authService);

await spotifyService.GetUserPlaylists();