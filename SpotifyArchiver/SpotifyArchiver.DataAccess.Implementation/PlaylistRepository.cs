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

        public async Task<List<Playlist>> FetchAllAsync()
        {
            return await context.Playlists.ToListAsync();
        }

        public async Task<Playlist?> GetBySpotifyIdAsync(string playlistId)
        {
            return await context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefaultAsync(p => p.SpotifyId == playlistId);
        }

        public async Task<Playlist?> GetByIdAsync(int id)
        {
            return await context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefaultAsync(p => p.PlaylistId == id);
        }
    }
}
