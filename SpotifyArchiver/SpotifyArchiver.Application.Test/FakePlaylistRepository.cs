using SpotifyAPI.Web;
using SpotifyArchiver.DataAccess.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.Application.Test
{
    public class FakePlaylistRepository: IPlaylistRepository
    {
        public List<Playlist> ArchivedPlaylists { get; } = new();

        public Task AddAsync(Playlist playlist)
        {
            ArchivedPlaylists.Add(playlist);
            return Task.CompletedTask;
        }

        public Task<List<Playlist>> FetchAllAsync()
        {
            return Task.FromResult(ArchivedPlaylists);
        }

        public Task<Playlist?> FetchByIdAsync(int playlistId)
        {
            return Task.FromResult(ArchivedPlaylists.FirstOrDefault(p => p.PlaylistId == playlistId));
        }

        public async Task RemovePlaylistByIdAsync(int playlistId)
        {
            var targetPlaylist = await FetchByIdAsync(playlistId);

            if (targetPlaylist == null)
            {
                throw new OperationCanceledException($"Unable to find archived playlist with id {playlistId}");
            }

            ArchivedPlaylists.Remove(targetPlaylist);
        }
    }
}
