using SpotifyArchiver.Application.Abstraction;

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
            operationHandler.AddOperation("Archive Playlist", "Archive a playlist by providing its id.", async Task () => await ArchivePlaylist(spotifyService));
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
                Console.WriteLine($"Id: {playlist.Id}\n{playlist.Name} - {playlist.SongCount} tracks\n");
            }
        }

        private static async Task ArchivePlaylist(ISpotifyService spotifyService)
        {
            Console.WriteLine("Please provide the id of the playlist you would like to archive.\n");
            var playlistId = Console.ReadLine();
            if (string.IsNullOrEmpty(playlistId))
            {
                Console.WriteLine("Playlist id cannot be empty.");
                return;
            }
            
            await spotifyService.ArchivePlaylistAsync(playlistId);
            Console.WriteLine($"Playlist {playlistId} has been successfully archived.\n");
        }
    }
}
