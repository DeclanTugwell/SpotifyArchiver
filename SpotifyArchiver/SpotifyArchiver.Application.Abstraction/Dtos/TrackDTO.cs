namespace SpotifyArchiver.Application.Abstraction.Dtos;

public class TrackDTO
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Artist { get; set; }
    public int? DurationMs { get; set; }
}
