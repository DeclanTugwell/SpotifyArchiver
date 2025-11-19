using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.DataAccess.Abstraction
{
    public interface IPlaylistRepository
    {
        Task AddAsync(Playlist playlist);
        
        // Gets all playlists from the database.
        Task<List<Playlist>> GetAllAsync();
    }
}
