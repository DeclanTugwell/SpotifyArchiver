namespace SpotifyArchiver.DataAccess.Abstraction.entities
{
    public class Track
    {
        public int TrackId { get; set; }
        public required string SpotifyId { get; set; }
        public required string Name { get; set; }
        public required string ArtistName { get; set; }
        public required string SpotifyUri { get; set; }

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
