using Microsoft.EntityFrameworkCore;
using SpotifyArchiver.Domain;

namespace SpotifyArchiver.DataAccess.Implementation
{
    public class SpotifyArchiverDbContext : DbContext
    {
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Track> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=spotify_archive.db");
        }
    }
}
