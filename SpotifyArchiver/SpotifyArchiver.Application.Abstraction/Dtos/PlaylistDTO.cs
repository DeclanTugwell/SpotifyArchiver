using System.Collections.Generic;

namespace SpotifyArchiver.Application.Abstraction.Dtos;

public class PlaylistDTO
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool Public { get; set; }
    public bool Collaborative { get; set; }
    public UserDTO? Owner { get; set; }
    public List<ImageDTO>? Images { get; set; }
    public List<TrackDTO>? Tracks { get; set; }
    public string? SnapshotId { get; set; }
    public string? Uri { get; set; }
}
