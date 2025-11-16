namespace SpotifyArchiver.DataAccess.Abstraction.entities
{
    public class Track
    {
        public int TrackId { get; set; } // Local DB Primary Key
        public string SpotifyId { get; set; } // Spotify Track ID
        public string Name { get; set; }
        public string ArtistName { get; set; }
        public string SpotifyUri { get; set; }

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
