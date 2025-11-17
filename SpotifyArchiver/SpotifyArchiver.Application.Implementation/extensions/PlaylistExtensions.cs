using SpotifyAPI.Web;
using SpotifyArchiver.Domain;

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
                    {
                        SpotifyId = playlist.Id,
                        Name = playlist.Name ?? "N/A",
                    });
                }
            }
        }
    }
}
