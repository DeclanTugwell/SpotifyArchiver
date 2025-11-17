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
            return await context.Playlists.ToListAsync();
        }
    }
}
