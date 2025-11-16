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
    }
}
