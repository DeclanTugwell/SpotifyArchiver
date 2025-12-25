using Microsoft.AspNetCore.Components;
using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.Presentation.Playlists.Components
{
    public partial class DataView
    {
        [Parameter] public List<Playlist> Playlists { get; set; } = [];
        [Parameter] public List<Track> Tracks { get; set; } = [];
        [Parameter] public string DisplayType { get; set; } = string.Empty;
        [Parameter] public EventCallback<Playlist> OnPlaylistSelected { get; set; }
        [Parameter] public EventCallback<Track> OnTrackSelected { get; set; }

        private Playlist? SelectedPlaylist { get; set; }
        private Track? SelectedTrack { get; set; }

        public async Task PlaylistSelected(Playlist playlist)
        {
            SelectedPlaylist = playlist;
            await OnPlaylistSelected.InvokeAsync(playlist);
            StateHasChanged();
        }

        public async Task TrackSelected(Track track)
        {
            SelectedTrack = track;
            await OnTrackSelected.InvokeAsync(track);
            StateHasChanged();
        }
    }
}
