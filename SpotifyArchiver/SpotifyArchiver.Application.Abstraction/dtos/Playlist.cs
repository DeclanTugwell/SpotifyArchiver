namespace SpotifyArchiver.Application.Abstraction.dtos;

public record Playlist
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Owner { get; init; }
    public required IEnumerable<Track> Tracks { get; init; }
}

public record Track
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required IEnumerable<string> Artists { get; init; }
}