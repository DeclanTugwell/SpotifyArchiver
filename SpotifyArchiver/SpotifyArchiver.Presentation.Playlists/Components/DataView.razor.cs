using Microsoft.AspNetCore.Components;

namespace SpotifyArchiver.Presentation.Playlists.Components
{
    public partial class DataView
    {
        [Parameter]
        public string Contents { get; set; } = string.Empty;
    }
}
