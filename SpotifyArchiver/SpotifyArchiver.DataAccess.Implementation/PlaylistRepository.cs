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

        public async Task<Playlist?> FetchByIdAsync(int playlistId)
        {
            return await context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);
        }

        public async Task DeletePlaylistAsync(int playlistId)
        {
            var playlist = await context.Playlists.FindAsync(playlistId);
            if (playlist != null)
            {
                context.Playlists.Remove(playlist);
                await context.SaveChangesAsync();
            }
        }
    }
}
