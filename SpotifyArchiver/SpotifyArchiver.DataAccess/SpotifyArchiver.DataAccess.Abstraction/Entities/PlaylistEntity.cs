namespace SpotifyArchiver.DataAccess.Abstraction.Entities;

public class PlaylistEntity
{
    public long Id { get; set; }
    public required string SpotifyId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Owner { get; set; }
    public required string SnapshotId { get; set; }
    public required string Uri { get; set; }
    
    public List<PlaylistTrackEntity> PlaylistTracks { get; set; } = new();
}