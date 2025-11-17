using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation.Features.Commands;
using SpotifyArchiver.Application.Implementation;
using SpotifyArchiver.DataAccess.Implementation;
using SpotifyArchiver.DataAccess.Abstraction;
using Microsoft.EntityFrameworkCore; // Add this line

namespace SpotifyArchiver.Presentation
{
    public class OperationHandler
    {
        private readonly List<Operation> _operations = [];

        public static OperationHandler Build(ISpotifyService spotifyService)
        {
            var operationHandler = new OperationHandler();
            operationHandler.AddOperation("Help", "Show descriptions for all available operations.", Task () => ShowHelp(operationHandler._operations));
            operationHandler.AddOperation("List Playlists", "Fetch and display all playlists from the authenticated Spotify account.", async Task () => await QueryPlaylists(spotifyService));
            operationHandler.AddOperation("Archive Playlist", "Fetch and display all playlists from the authenticated Spotify account, then archive the selected playlist.", async Task () => await ArchivePlaylist(spotifyService));
            return operationHandler;
        }

        public void ShowAvailableOperations()
        {
            Console.WriteLine("Available Operations:\n");

            for (var count = 0; _operations.Count > count; count ++)
            {
                Console.WriteLine($"{count}. {_operations[count].Name}\n");
            }
        }

        public async Task AwaitOperation()
        {
            var operationIndex = Console.ReadLine();

            var operation = _operations.ElementAtOrDefault(int.Parse(operationIndex ?? "-1"));

            if (operation is null)
            {
                Console.WriteLine("Invalid operation selected. Please try again.\n");
                await AwaitOperation();
            }
            else
            {
                await operation.Execute();
            }
        }
        private OperationHandler()
        {

        }

        private void AddOperation(string operationName, string operationDescription, Func<Task> operation)
        {
            _operations.Add(new Operation(operationName, operationDescription, operation));
        }

        private static Task ShowHelp(List<Operation> operations)
        {
            Console.WriteLine("Operation Descriptions:\n");
            foreach (var operation in operations)
            {
                Console.WriteLine($"{operation.Name}: {operation.Description}\n");
            }

            return Task.CompletedTask;
        }

        private static async Task QueryPlaylists(ISpotifyService spotifyService)
        {
            var playlists = await spotifyService.GetPlaylistsAsync();
            Console.WriteLine("Your Playlists:\n");
            foreach (var playlist in playlists)
            {
                Console.WriteLine($"Id: {playlist.SpotifyId}\n{playlist.Name}\n");
            }
        }

        private static async Task ArchivePlaylist(ISpotifyService spotifyService)
        {
            var playlists = await spotifyService.GetPlaylistsAsync();
            Console.WriteLine("Select a playlist to archive:\n");
            for (var i = 0; i < playlists.Count; i++)
            {
                Console.WriteLine($"{i}. {playlists[i].Name}");
            }

            var playlistIndex = Console.ReadLine();
            var playlist = playlists.ElementAtOrDefault(int.Parse(playlistIndex ?? "-1"));

            if (playlist is null)
            {
                Console.WriteLine("Invalid playlist selected. Please try again.\n");
                await ArchivePlaylist(spotifyService);
            }
            else
            {
                // Directly instantiate and call the command handler
                var dbContext = new SpotifyArchiverDbContext();
                await dbContext.Database.MigrateAsync(); // Use MigrateAsync for async operation
                var playlistRepository = new PlaylistRepository(dbContext);
                var handler = new ArchivePlaylistCommandHandler(spotifyService, playlistRepository);
                var command = new ArchivePlaylistCommand(playlist.SpotifyId);
                await handler.HandleAsync(command);
                Console.WriteLine($"Playlist '{playlist.Name}' archived successfully.");
            }
        }
    }
}
