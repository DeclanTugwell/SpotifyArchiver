using Microsoft.EntityFrameworkCore;
using SpotifyArchiver.DataAccess.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.DataAccess.Implementation
{
    public class PlaylistRepository(MusicDbContext context): IPlaylistRepository
    {
        public async Task AddAsync(Playlist playlist)
        {
            context.Playlists.Add(playlist);
            await context.SaveChangesAsync();
        }

        public async Task<List<Playlist>> GetAllAsync()
        {
            // Gets all playlists from the database including their tracks.
            return await context.Playlists.Include(p => p.Tracks).ToListAsync();
        }
    }
}
