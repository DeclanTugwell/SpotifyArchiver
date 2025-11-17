using SpotifyArchiver.Application.Abstraction;

namespace SpotifyArchiver.Presentation;

public class ConsoleController
{
    private readonly ISpotifyService _spotifyService;

    public ConsoleController(ISpotifyService spotifyService)
    {
        _spotifyService = spotifyService;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Welcome to Spotify Archiver!");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. Get all playlists");
            Console.WriteLine("0. Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await GetPlaylistsAsync(cancellationToken);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private async Task GetPlaylistsAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Fetching playlists...");
        var playlists = await _spotifyService.GetPlaylistsAsync(cancellationToken);
        
        foreach (var playlist in playlists)
        {
            Console.WriteLine($"- {playlist.Name} (by {playlist.Owner})");
        }
    }
}