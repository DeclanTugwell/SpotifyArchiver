using SpotifyAPI.Web;
using SpotifyArchiver.Application.Abstraction;

namespace SpotifyArchiver.Application.Implementation.extensions
{
    public static class PlaylistExtensions
    {
        public static void AddPlaylistPage(this List<Playlist> playlists, List<FullPlaylist> playlistPage)
        {
            foreach (var playlist in playlistPage)
            {
                if (playlist.Id != null)
                {
                    playlists.Add(new Playlist
                    (
                        playlist.Id,
                        playlist.Name?? "N/A",
                        playlist.Tracks?.Total ?? 0
                    ));
                }
            }
        }
    }
}
