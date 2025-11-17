using SpotifyArchiver.Domain;

namespace SpotifyArchiver.DataAccess.Abstraction
{
    public interface IPlaylistRepository
    {
        Task AddPlaylistAsync(Playlist playlist);
    }
}
