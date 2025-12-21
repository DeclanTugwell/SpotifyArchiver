using Microsoft.AspNetCore.Components;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.Presentation.Playlists.Pages
{
    public partial class Home
    {
        [Inject] private ISpotifyService SpotifyService { get; init; } = default!;
        [Inject] private IPlaylistRepository PlaylistRepository { get; init; } = default!;

        private string _dataViewContents
        {
            get;
            set
            {
                field = value;
                StateHasChanged();
            }
        } = string.Empty;

        public async Task<bool> TryAuthenticate(CancellationToken token)
        {
            return await SpotifyService.TryAuthenticateAsync(token);
        }

        private void AppendToOutput(string output)
        {
            _dataViewContents += output + Environment.NewLine;
        }

        private void ClearOutput() => _dataViewContents = string.Empty;


        private async Task QueryPlaylists()
        {
            ClearOutput();
            var playlists = await SpotifyService.GetPlaylistsAsync();
            AppendToOutput("Your Playlists:\n\n");
            foreach (var playlist in playlists)
            {
                AppendToOutput($"Id: {playlist.SpotifyId}\n{playlist.Name}\n");
            }
        }

        private async Task ArchivePlaylist(Playlist targetPlaylist)
        {
            ClearOutput();
            var playlistId = targetPlaylist.SpotifyId;
            await SpotifyService.ArchivePlaylist(playlistId);

            AppendToOutput("Playlist archived successfully.\n");
        }

        private async Task QueryArchivedPlaylists()
        {
            ClearOutput();
            var playlists = await PlaylistRepository.FetchAllAsync();

            if (playlists.Any() == false)
            {
                AppendToOutput("No Playlists Archived.");
                return;
            }

            AppendToOutput("Your Archived Playlists:\n");

            foreach (var playlist in playlists)
            {
                AppendToOutput($"Id: {playlist.PlaylistId}\nSpotifyId: {playlist.SpotifyId}\n{playlist.Name}\n");
            }
        }

        private async Task FetchAllSongsFromArchivedPlaylist(Playlist targetPlaylist)
        {
            ClearOutput();
            var playlistId = targetPlaylist.PlaylistId;
            var playlist = await PlaylistRepository.FetchByIdAsync(playlistId);

            if (playlist == null)
            {
                AppendToOutput("No playlist found matching that ID.");
                return;
            }

            AppendToOutput($"Songs in Playlist: {playlist.Name}\n");

            var count = 0;
            foreach (var track in playlist.Tracks)
            {
                AppendToOutput($"{count}. {track.Name} by {track.ArtistName}\n{track.SpotifyUri}\n\n");
                count++;
            }
        }

        private async Task RemovedArchivedPlaylist(Playlist targetPlaylist)
        {
            ClearOutput();
            var playlistId = targetPlaylist.PlaylistId;
            await PlaylistRepository.RemovePlaylistByIdAsync(playlistId);

            AppendToOutput($"Playlist Removed: {playlistId}");
        }
    }
}
