using SpotifyArchiver.DataAccess.Abstraction;
using SpotifyArchiver.Domain;

namespace SpotifyArchiver.DataAccess.Implementation
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly SpotifyArchiverDbContext _context;

        public PlaylistRepository(SpotifyArchiverDbContext context)
        {
            _context = context;
        }

        public async Task AddPlaylistAsync(Playlist playlist)
        {
            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();
        }
    }
}
