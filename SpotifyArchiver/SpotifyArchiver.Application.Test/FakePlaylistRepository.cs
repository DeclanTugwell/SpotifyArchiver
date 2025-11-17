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

        public Task DeletePlaylistAsync(int playlistId)
        {
            var playlist = ArchivedPlaylists.FirstOrDefault(p => p.PlaylistId == playlistId);
            if (playlist != null)
            {
                ArchivedPlaylists.Remove(playlist);
            }
            return Task.CompletedTask;
        }
    }
}
