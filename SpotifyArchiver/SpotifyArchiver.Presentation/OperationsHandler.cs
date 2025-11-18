using System;
using System.Threading.Tasks;
using SpotifyArchiver.Application.Abstraction;

namespace SpotifyArchiver.Presentation
{
    public class OperationsHandler
    {
        private readonly ISpotifyService _spotifyService;

        public OperationsHandler(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        public async Task ExecuteOperationAsync(string operation)
        {
            switch (operation)
            {
                case "get-playlists":
                    var playlists = await _spotifyService.GetPlaylistsAsync();
                    foreach (var playlist in playlists)
                    {
                        Console.WriteLine($"- {playlist.Name} ({playlist.Id})");
                    }
                    break;
                default:
                    Console.WriteLine($"Unknown operation: {operation}");
                    break;
            }
        }
    }
}
