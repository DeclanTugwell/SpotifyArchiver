using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.DataAccess.Abstraction
{
    public interface IPlaylistRepository
    {
        Task AddAsync(Playlist playlist);
    }
}
