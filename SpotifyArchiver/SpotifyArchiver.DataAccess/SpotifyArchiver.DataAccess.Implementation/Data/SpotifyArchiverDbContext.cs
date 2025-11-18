using Microsoft.EntityFrameworkCore;
using SpotifyArchiver.DataAccess.Abstraction.Entities;

namespace SpotifyArchiver.DataAccess.Implementation.Data;

public class SpotifyArchiverDbContext : DbContext
{
    public SpotifyArchiverDbContext(DbContextOptions<SpotifyArchiverDbContext> options) : base(options)
    {
    }

    public DbSet<PlaylistEntity> Playlists { get; set; }
    public DbSet<TrackEntity> Tracks { get; set; }
    public DbSet<PlaylistTrackEntity> PlaylistTracks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlaylistEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SpotifyId).IsUnique();
        });

        modelBuilder.Entity<TrackEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SpotifyId).IsUnique();
        });

        modelBuilder.Entity<PlaylistTrackEntity>()
            .HasKey(pt => new { pt.PlaylistId, pt.TrackId });

        modelBuilder.Entity<PlaylistTrackEntity>()
            .HasOne(pt => pt.Playlist)
            .WithMany(p => p.PlaylistTracks)
            .HasForeignKey(pt => pt.PlaylistId);

        modelBuilder.Entity<PlaylistTrackEntity>()
            .HasOne(pt => pt.Track)
            .WithMany(t => t.PlaylistTracks)
            .HasForeignKey(pt => pt.TrackId);
    }
}