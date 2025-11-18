namespace SpotifyArchiver.DataAccess.Abstraction.Entities;

public class TrackEntity
{
    public long Id { get; set; }
    public required string SpotifyId { get; set; }
    public required string Name { get; set; }
    public required string Artists { get; set; }
    public required string Album { get; set; }
    public int DurationMs { get; set; }
    public required string Uri { get; set; }
    
    public List<PlaylistTrackEntity> PlaylistTracks { get; set; } = new();
}