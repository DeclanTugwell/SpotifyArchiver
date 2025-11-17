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

        public async Task RemovePlaylistByIdAsync(int playlistId)
        {
            var targetPlaylist = await FetchByIdAsync(playlistId);

            if (targetPlaylist == null)
            {
                throw new OperationCanceledException($"Unable to find archived playlist with id {playlistId}");
            }

            context.Playlists.Remove(targetPlaylist);
            await context.SaveChangesAsync();
        }
    }
}
