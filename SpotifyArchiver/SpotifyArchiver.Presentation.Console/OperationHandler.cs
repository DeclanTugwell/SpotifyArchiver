using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction;
using SysConsole = System.Console;

namespace SpotifyArchiver.Presentation.Console
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
            operationHandler.AddOperation("List Songs from Archived Playlist", "Fetches and displays all songs from a specified archived playlist.", async Task () => await operationHandler.FetchAllSongsFromArchivedPlaylist());
            operationHandler.AddOperation("Remove Archived Playlist", "Removes an archived playlist from the local database.", async Task () => await operationHandler.RemovedArchivedPlaylist());
            return operationHandler;
        }

        public void ShowAvailableOperations()
        {
            SysConsole.WriteLine("Available Operations:\n\n");

            for (var count = 0; _operations.Count > count; count++)
            {
                SysConsole.WriteLine($"{count}. {_operations[count].Name}\n");
            }
        }

        public async Task AwaitOperation()
        {
            var operationIndex = SysConsole.ReadLine();

            var operation = _operations.ElementAtOrDefault(int.Parse(operationIndex ?? "-1"));

            if (operation is null)
            {
                SysConsole.WriteLine("Invalid operation selected. Please try again.\n");
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

        private OperationHandler(ISpotifyService spotifyService, IPlaylistRepository playlistRepository)
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
            SysConsole.WriteLine("Operation Descriptions:\n");
            foreach (var operation in operations)
            {
                SysConsole.WriteLine($"{operation.Name}: {operation.Description}\n");
            }

            return Task.CompletedTask;
        }

        private async Task QueryPlaylists()
        {
            var playlists = await _spotifyService.GetPlaylistsAsync();
            SysConsole.WriteLine("Your Playlists:\n\n");
            foreach (var playlist in playlists)
            {
                SysConsole.WriteLine($"Id: {playlist.SpotifyId}\n{playlist.Name}\n");
            }
        }

        private async Task ArchivePlaylist()
        {
            SysConsole.WriteLine("Enter Playlist Id to Archive\n");

            var playlistId = "";
            while (string.IsNullOrEmpty(playlistId))
            {
                playlistId = SysConsole.ReadLine();
            }

            await _spotifyService.ArchivePlaylist(playlistId);

            SysConsole.WriteLine("Playlist archived successfully.\n");
        }

        private async Task QueryArchivedPlaylists()
        {
            var playlists = await _playlistRepository.FetchAllAsync();

            if (playlists.Any() == false)
            {
                SysConsole.WriteLine("No Playlists Archived.");
                return;
            }

            SysConsole.WriteLine("Your Archived Playlists:\n");

            foreach (var playlist in playlists)
            {
                SysConsole.WriteLine($"Id: {playlist.PlaylistId}\nSpotifyId: {playlist.SpotifyId}\n{playlist.Name}\n");
            }
        }

        private async Task FetchAllSongsFromArchivedPlaylist()
        {
            await QueryArchivedPlaylists();

            SysConsole.WriteLine("Enter Archived Playlist Id\n");

            int playlistId = -1;
            while (playlistId < 0)
            {
                playlistId = int.Parse(SysConsole.ReadLine() ?? "-1");
            }

            var playlist = await _playlistRepository.FetchByIdAsync(playlistId);

            if (playlist == null)
            {
                SysConsole.WriteLine("No playlist found matching that ID.");
                return;
            }

            SysConsole.WriteLine($"Songs in Playlist: {playlist.Name}\n");

            var count = 0;
            foreach (var track in playlist.Tracks)
            {
                SysConsole.WriteLine($"{count}. {track.Name} by {track.ArtistName}\n{track.SpotifyUri}\n\n");
                count++;
            }
        }

        private async Task RemovedArchivedPlaylist()
        {
            await QueryArchivedPlaylists();

            SysConsole.WriteLine("Enter Archived Playlist Id to Remove\n");

            int playlistId = -1;
            while (playlistId < 0)
            {
                playlistId = int.Parse(SysConsole.ReadLine() ?? "-1");
            }

            await _playlistRepository.RemovePlaylistByIdAsync(playlistId);

            SysConsole.WriteLine($"Playlist Removed: {playlistId}");
        }
    }
}
