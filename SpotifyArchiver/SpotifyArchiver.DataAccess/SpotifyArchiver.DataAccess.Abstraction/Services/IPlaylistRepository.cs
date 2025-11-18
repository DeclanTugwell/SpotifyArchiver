using SpotifyArchiver.DataAccess.Abstraction.Entities;

namespace SpotifyArchiver.DataAccess.Abstraction.Services
{
    public interface IPlaylistRepository
    {
        Task SavePlaylistAsync(PlaylistEntity playlist);
    }
}