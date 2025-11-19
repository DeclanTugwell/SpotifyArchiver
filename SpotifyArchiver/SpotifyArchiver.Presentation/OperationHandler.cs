using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction;

namespace SpotifyArchiver.Presentation
{
    public class OperationHandler
    {
        private readonly List<Operation> _operations = [];
        private readonly ISpotifyService _spotifyService;
        private readonly IPlaylistRepository _playlistRepository;

        public static OperationHandler Build(ISpotifyService spotifyService, IPlaylistRepository playlistRepository)
        {
            var operationHandler = new OperationHandler(spotifyService, playlistRepository);
            operationHandler.AddOperation("Help", "Show descriptions for all available operations.", Task () => ShowHelp(operationHandler._operations));
            operationHandler.AddOperation("List Playlists", "Fetch and display all playlists from the authenticated Spotify account.", async Task () => await operationHandler.QueryPlaylists());
            operationHandler.AddOperation("Archive Playlist", "Archives playlist and all tracks into local database.", async Task () => await operationHandler.ArchivePlaylist());
            operationHandler.AddOperation("List Archived Playlists", "Lists all the playlists archived in the local database.", async Task () => await operationHandler.QueryArchivedPlaylists());
            operationHandler.AddOperation("List Archived Playlist Songs", "Lists all the songs in an archived playlist.", async Task () => await operationHandler.QueryArchivedPlaylistSongs());
            return operationHandler;
        }

        public void ShowAvailableOperations()
        {
            Console.WriteLine("Available Operations:\n\n");

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

        private OperationHandler(ISpotifyService spotifyService , IPlaylistRepository playlistRepository)
        {
            _spotifyService = spotifyService;
            _playlistRepository = playlistRepository;
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

        private async Task QueryPlaylists()
        {
            var playlists = await _spotifyService.GetPlaylistsAsync();
            Console.WriteLine("Your Playlists:\n\n");
            foreach (var playlist in playlists)
            {
                Console.WriteLine($"Id: {playlist.SpotifyId}\n{playlist.Name}\n");
            }
        }

        private async Task ArchivePlaylist()
        {
            Console.WriteLine("Enter Playlist Id to Archive\n");

            var playlistId = "";
            while (string.IsNullOrEmpty(playlistId))
            {
                playlistId = Console.ReadLine();
            }

            await _spotifyService.ArchivePlaylist(playlistId);

            Console.WriteLine("Playlist archived successfully.\n");
        }

        private async Task QueryArchivedPlaylists()
        {
            var playlists = await _playlistRepository.FetchAllAsync();

            Console.WriteLine("Your Archived Playlists:\n");

            foreach (var playlist in playlists)
            {
                Console.WriteLine($"Id: {playlist.PlaylistId}\nSpotifyId: {playlist.SpotifyId}\n{playlist.Name}\n");
            }
        }
        
        private async Task QueryArchivedPlaylistSongs()
        {
            Console.WriteLine("Enter Playlist Id to view songs\n");

            var playlistIdStr = "";
            while (string.IsNullOrEmpty(playlistIdStr))
            {
                playlistIdStr = Console.ReadLine();
            }

            if (int.TryParse(playlistIdStr, out var playlistId))
            {
                var playlist = await _playlistRepository.GetByIdAsync(playlistId);

                if (playlist is null)
                {
                    Console.WriteLine("Playlist not found.\n");
                    return;
                }

                Console.WriteLine($"Songs in {playlist.Name}:\n");

                foreach (var track in playlist.Tracks)
                {
                    Console.WriteLine($"Id: {track.SpotifyId}");
                    Console.WriteLine($"Name: {track.Name}");
                    Console.WriteLine($"Artist: {track.ArtistName}");
                    Console.WriteLine($"Uri: {track.SpotifyUri}\n");
                }
            }
            else
            {
                Console.WriteLine("Invalid Playlist Id.\n");
            }
        }
    }
}
