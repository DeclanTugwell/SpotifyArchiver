namespace SpotifyArchiver.DataAccess.Abstraction.entities
{
    public class Playlist
    {
        public int PlaylistId { get; set; } 
        public string SpotifyId { get; set; } 
        public string Name { get; set; }
        public string SpotifyUri { get; set; }
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
