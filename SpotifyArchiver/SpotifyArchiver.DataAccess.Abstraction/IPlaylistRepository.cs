using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.DataAccess.Abstraction
{
    public interface IPlaylistRepository
    {
        Task AddAsync(Playlist playlist);

        Task<List<Playlist>> FetchAllAsync();
        Task<Playlist?> GetBySpotifyIdAsync(string playlistId);
        Task<Playlist?> GetByIdAsync(int id);
    }
}
