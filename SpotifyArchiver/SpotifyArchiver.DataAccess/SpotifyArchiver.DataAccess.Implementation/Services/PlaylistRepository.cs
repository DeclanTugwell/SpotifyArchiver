using Microsoft.EntityFrameworkCore;
using SpotifyArchiver.DataAccess.Abstraction.Entities;
using SpotifyArchiver.DataAccess.Abstraction.Services;
using SpotifyArchiver.DataAccess.Implementation.Data;

namespace SpotifyArchiver.DataAccess.Implementation.Services;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly SpotifyArchiverDbContext _context;

    public PlaylistRepository(SpotifyArchiverDbContext context)
    {
        _context = context;
    }

    public async Task SavePlaylistAsync(PlaylistEntity playlist)
    {
        // Check if playlist already exists
        var existingPlaylist = await _context.Playlists
            .Include(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
            .FirstOrDefaultAsync(p => p.SpotifyId == playlist.SpotifyId);

        if (existingPlaylist == null)
        {
            // Add new playlist
            _context.Playlists.Add(playlist);
        }
        else
        {
            // Update existing playlist properties
            existingPlaylist.Name = playlist.Name;
            existingPlaylist.Description = playlist.Description;
            existingPlaylist.Owner = playlist.Owner;
            existingPlaylist.SnapshotId = playlist.SnapshotId;
            existingPlaylist.Uri = playlist.Uri;

            // Handle tracks
            foreach (var playlistTrack in playlist.PlaylistTracks)
            {
                if (playlistTrack.Track == null)
                {
                    continue;
                }
                
                var existingTrack = await _context.Tracks
                    .FirstOrDefaultAsync(t => t.SpotifyId == playlistTrack.Track.SpotifyId);

                if (existingTrack == null)
                {
                    // Add new track
                    _context.Tracks.Add(playlistTrack.Track);
                }
                else
                {
                    // Update existing track properties
                    existingTrack.Name = playlistTrack.Track.Name;
                    existingTrack.Artists = playlistTrack.Track.Artists;
                    existingTrack.Album = playlistTrack.Track.Album;
                    existingTrack.DurationMs = playlistTrack.Track.DurationMs;
                    existingTrack.Uri = playlistTrack.Track.Uri;
                    playlistTrack.Track = existingTrack; // Ensure the relationship uses the existing track
                }

                // Check if the playlist-track relationship already exists
                var existingPlaylistTrack = existingPlaylist.PlaylistTracks
                    .FirstOrDefault(pt => pt.Track != null && pt.Track.SpotifyId == playlistTrack.Track.SpotifyId);

                if (existingPlaylistTrack == null)
                {
                    // Add new relationship
                    existingPlaylist.PlaylistTracks.Add(playlistTrack);
                }
                else
                {
                    // Update existing relationship properties if necessary (e.g., AddedAt)
                    existingPlaylistTrack.AddedAt = playlistTrack.AddedAt;
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}