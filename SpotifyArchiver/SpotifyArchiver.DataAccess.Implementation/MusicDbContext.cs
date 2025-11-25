using Microsoft.EntityFrameworkCore;
using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.DataAccess.Implementation
{
    public class MusicDbContext(DbContextOptions<MusicDbContext> options) : DbContext(options)
    {
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Track> Tracks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.Tracks)
                .WithMany(t => t.Playlists)
                .UsingEntity(join =>
                    join.ToTable("PlaylistTrack")
                );
        }
    }
}
