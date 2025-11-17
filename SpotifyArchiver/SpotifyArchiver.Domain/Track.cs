namespace SpotifyArchiver.Domain
{
    public class Track
    {
        public int Id { get; set; }
        public string? SpotifyId { get; set; }
        public string? Name { get; set; }
        public string? Artist { get; set; }
        public string? Album { get; set; }
        public int PlaylistId { get; set; }
        public Playlist? Playlist { get; set; }
    }
}

