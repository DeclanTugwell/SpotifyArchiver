using SpotifyArchiver.DataAccess.Abstraction.Entities;
using SpotifyArchiver.DataAccess.Abstraction.Services;

namespace SpotifyArchiver.Application.Test.Fakes
{
    public class FakePlaylistRepository : IPlaylistRepository
    {
        public List<PlaylistEntity> SavedPlaylists { get; } = new List<PlaylistEntity>();

        public Task SavePlaylistAsync(PlaylistEntity playlist)
        {
            SavedPlaylists.Add(playlist);
            return Task.CompletedTask;
        }
    }
}
