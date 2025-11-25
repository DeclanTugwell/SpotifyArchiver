namespace SpotifyArchiver.DataAccess.Abstraction.entities
{
    public class Playlist
    {
        public int PlaylistId { get; set; } 
        public required string SpotifyId { get; set; } 
        public required string Name { get; set; }
        public required string SpotifyUri { get; set; }
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
