namespace SpotifyArchiver.Domain
{
    public class Playlist
    {
        public int Id { get; set; }
        public string? SpotifyId { get; set; }
        public string? Name { get; set; }
        public List<Track> Tracks { get; set; } = new List<Track>();
    }
}
