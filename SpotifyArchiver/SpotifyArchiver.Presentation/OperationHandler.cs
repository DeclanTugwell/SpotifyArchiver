using SpotifyArchiver.Application.Abstraction;

namespace SpotifyArchiver.Presentation
{
    public class OperationHandler
    {
        private readonly List<Operation> _operations = [];
        private readonly ISpotifyService _spotifyService;

        public static OperationHandler Build(ISpotifyService spotifyService)
        {
            var operationHandler = new OperationHandler(spotifyService);
            operationHandler.AddOperation("Help", "Show descriptions for all available operations.", Task () => ShowHelp(operationHandler._operations));
            operationHandler.AddOperation("List Playlists", "Fetch and display all playlists from the authenticated Spotify account.", async Task () => await QueryPlaylists(spotifyService));
            operationHandler.AddOperation("Archive Playlist", "Archives playlist and all tracks into local database.", async Task () => await ArchivePlaylist(spotifyService));
            operationHandler.AddOperation("Show Archived Playlists", "Displays all playlists that have been archived in the local database.", async Task () => await ShowArchivedPlaylists(spotifyService));
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
        
        public async Task<bool> TryAuthenticate(CancellationToken token)
        {
            return await _spotifyService.TryAuthenticateAsync(token);
        }
        
        private OperationHandler(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
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
                Console.WriteLine($"Id: {playlist.SpotifyId}\n{playlist.Name}");
            }
        }
        
        private static async Task ArchivePlaylist(ISpotifyService spotifyService)
        {
            Console.WriteLine("Enter Playlist Id to Archive\n");
        
            var playlistId = "";
            while (string.IsNullOrEmpty(playlistId))
            {
                playlistId = Console.ReadLine();
            }
        
            await spotifyService.ArchivePlaylist(playlistId);
        
            Console.WriteLine("Playlist archived successfully.\n");
        }
        
        private static async Task ShowArchivedPlaylists(ISpotifyService spotifyService)
        {
            var playlists = await spotifyService.GetArchivedPlaylistsAsync();
            Console.WriteLine("Archived Playlists:\n");
            foreach (var playlist in playlists)
            {
                Console.WriteLine($"Id: {playlist.PlaylistId}\nName: {playlist.Name}");
            }
        }
    }
}
