using Microsoft.AspNetCore.Components;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction.entities;
using System.Diagnostics;

namespace SpotifyArchiver.Presentation.Playlists.Pages
{
    public partial class Home
    {
        [Inject] private ISpotifyService SpotifyService { get; init; } = default!;
        [Inject] private IPlaylistRepository PlaylistRepository { get; init; } = default!;

        private List<Playlist> Playlists
        {
            get;
            set
            {
                field = value;
                SelectedDisplayMode = nameof(Playlists);
                StateHasChanged();
            }
        } = [];

        private List<Track> Tracks
        {
            get;
            set
            {
                field = value;
                SelectedDisplayMode = nameof(Tracks);
                StateHasChanged();
            }
        }

        private string SelectedDisplayMode
        {
            get;
            set
            {
                field = value;
                StateHasChanged();
            }
        } = nameof(Playlists);

        private string DataViewContents
        {
            get;
            set
            {
                field = value;
                StateHasChanged();
            }
        } = string.Empty;

        private Playlist? SelectedPlaylist { get; set; }

        private Track? SelectedTrack { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await SpotifyService.TryAuthenticateAsync(CancellationToken.None);
            ClearOutput();
        }
        
        private async Task QueryPlaylists()
        {
            ClearOutput();
            var playlists = await SpotifyService.GetPlaylistsAsync();
            Playlists = playlists;
            AppendToOutput("Your Playlists:\n\n");
            foreach (var playlist in playlists)
            {
                AppendToOutput($"Id: {playlist.SpotifyId}\n{playlist.Name}\n");
            }
        }

        private async Task ArchivePlaylist()
        {
            if (SelectedPlaylist == null)
            {
                return;
            }

            ClearOutput();
            var playlistId = SelectedPlaylist.SpotifyId;
            await SpotifyService.ArchivePlaylist(playlistId);

            AppendToOutput("Playlist archived successfully.\n");
        }

        private async Task QueryArchivedPlaylists()
        {
            ClearOutput();
            var playlists = await PlaylistRepository.FetchAllAsync();

            if (playlists.Any() == false)
            {
                Playlists = [];
                AppendToOutput("No Playlists Archived.");
                return;
            }

            Playlists = playlists;
            AppendToOutput("Your Archived Playlists:\n");

            foreach (var playlist in playlists)
            {
                AppendToOutput($"Id: {playlist.PlaylistId}\nSpotifyId: {playlist.SpotifyId}\n{playlist.Name}\n");
            }
        }

        private async Task FetchAllSongsFromArchivedPlaylist()
        {
            if (SelectedPlaylist == null)
            {
                return;
            }

            ClearOutput();
            var playlistId = SelectedPlaylist.PlaylistId;
            var playlist = await PlaylistRepository.FetchByIdAsync(playlistId);

            if (playlist == null)
            {
                AppendToOutput("No playlist found matching that ID.");
                return;
            }

            Tracks = playlist.Tracks.ToList();
            AppendToOutput($"Songs in Playlist: {playlist.Name}\n");

            var count = 0;
            foreach (var track in playlist.Tracks)
            {
                AppendToOutput($"{count}. {track.Name} by {track.ArtistName}\n{track.SpotifyUri}\n\n");
                count++;
            }
        }

        private async Task RemovedArchivedPlaylist()
        {
            if (SelectedPlaylist == null)
            {
                return;
            }
            
            ClearOutput();
            var playlistId = SelectedPlaylist.PlaylistId;
            await PlaylistRepository.RemovePlaylistByIdAsync(playlistId);
            await QueryArchivedPlaylists();
            AppendToOutput($"Playlist Removed: {playlistId}");
        }

        private void OnPlaylistSelected(Playlist playlist)
        {
            SelectedPlaylist = playlist;
        }

        private void OnTrackSelected(Track track)
        {
            SelectedTrack = track;
        }
        
        private void AppendToOutput(string output)
        {
            DataViewContents += output + Environment.NewLine;
        }

        private void ClearOutput() => DataViewContents = string.Empty;
    }
}
