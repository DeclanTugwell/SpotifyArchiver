namespace SpotifyArchiver.DataAccess.Abstraction.Entities;

public class PlaylistTrackEntity
{
    public long PlaylistId { get; set; }
    public PlaylistEntity? Playlist { get; set; }

    public long TrackId { get; set; }
    public TrackEntity? Track { get; set; }

    public DateTime AddedAt { get; set; }
}