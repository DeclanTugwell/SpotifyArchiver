using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.DataAccess.Abstraction
{
    public interface IPlaylistRepository
    {
        Task AddAsync(Playlist playlist);

        Task<List<Playlist>> FetchAllAsync();

        Task<Playlist?> FetchByIdAsync(int playlistId);

        Task DeletePlaylistAsync(int playlistId);
    }
}
